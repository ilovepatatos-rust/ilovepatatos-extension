#!/usr/bin/env pwsh
[CmdletBinding()]
param(
    [string]$Version,
    [string]$Script,
    [ValidateSet('Windows', 'Linux', 'All')][string]$ServerPlatform,
    [switch]$Publicize,
    [switch]$WhatIf,
    [string]$Repo = 'ilovepatatos-rust/rust-dependencies',
    [string]$ProjectRoot,
    [string]$ToolkitRoot
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-RdBootstrapMenuEntries {
    @(
        [pscustomobject]@{ Number = 1; Name = 'rust'; Channel = 'Production' }
        [pscustomobject]@{ Number = 2; Name = 'rust-staging'; Channel = 'Staging' }
        [pscustomobject]@{ Number = 3; Name = 'rust+oxide'; Channel = 'Production' }
        [pscustomobject]@{ Number = 4; Name = 'rust-staging+oxide'; Channel = 'Staging' }
        [pscustomobject]@{ Number = 5; Name = 'rust+carbon'; Channel = 'Production' }
        [pscustomobject]@{ Number = 6; Name = 'rust-staging+carbon'; Channel = 'Staging' }
        [pscustomobject]@{ Number = 7; Name = 'rust+oxide+carbon'; Channel = 'Production' }
        [pscustomobject]@{ Number = 8; Name = 'rust-staging+oxide+carbon'; Channel = 'Staging' }
        [pscustomobject]@{ Number = 9; Name = 'all'; Channel = $null }
    )
}

function Resolve-RdBootstrapVersionLabel {
    param([string]$Version)
    if ([string]::IsNullOrWhiteSpace($Version) -or $Version -eq 'latest') {
        return 'latest'
    }
    return $Version.TrimStart('v', 'V')
}

function Set-RdBootstrapGitHubTokenProcessCache {
    param([AllowNull()][AllowEmptyString()][string]$Token)
    # Share with the toolkit module / parallel children so credential helpers
    # are not invoked again in the same process tree.
    $env:RD_GITHUB_TOKEN_RESOLVED = '1'
    if ([string]::IsNullOrWhiteSpace($Token)) {
        $env:RD_GITHUB_TOKEN = ''
    } else {
        $env:RD_GITHUB_TOKEN = $Token
    }
}

function Get-RdBootstrapGitHubToken {
    if ($env:RD_GITHUB_TOKEN_RESOLVED -eq '1') {
        $resolved = $env:RD_GITHUB_TOKEN
        if ([string]::IsNullOrWhiteSpace($resolved)) { return $null }
        return [string]$resolved
    }

    if ($env:GH_TOKEN) {
        Set-RdBootstrapGitHubTokenProcessCache ([string]$env:GH_TOKEN)
        return [string]$env:GH_TOKEN
    }
    if ($env:GITHUB_TOKEN) {
        Set-RdBootstrapGitHubTokenProcessCache ([string]$env:GITHUB_TOKEN)
        return [string]$env:GITHUB_TOKEN
    }

    $gh = Get-Command gh -ErrorAction SilentlyContinue
    if ($gh) {
        try {
            $token = (& $gh.Source auth token 2>$null)
            if ($LASTEXITCODE -eq 0 -and $token) {
                $value = ([string]($token | Select-Object -First 1)).Trim()
                if (-not [string]::IsNullOrWhiteSpace($value)) {
                    Set-RdBootstrapGitHubTokenProcessCache $value
                    return $value
                }
            }
        } catch { }
    }

    $git = Get-Command git -ErrorAction SilentlyContinue
    if ($git) {
        $previousPrompt = $env:GIT_TERMINAL_PROMPT
        $previousGcmInteractive = $env:GCM_INTERACTIVE
        try {
            # Never pop a GCM / terminal login UI for optional bootstrap auth.
            $env:GIT_TERMINAL_PROMPT = '0'
            $env:GCM_INTERACTIVE = 'never'
            $input = "protocol=https`nhost=github.com`n`n"
            $output = $input | & $git.Source credential fill 2>$null
            if ($LASTEXITCODE -eq 0 -and $output) {
                foreach ($line in @($output)) {
                    if ($line -match '^password=(?<token>.+)$') {
                        $value = $Matches['token'].Trim()
                        if (-not [string]::IsNullOrWhiteSpace($value)) {
                            Set-RdBootstrapGitHubTokenProcessCache $value
                            return $value
                        }
                    }
                }
            }
        } catch {
        } finally {
            if ($null -eq $previousPrompt) {
                Remove-Item Env:GIT_TERMINAL_PROMPT -ErrorAction SilentlyContinue
            } else {
                $env:GIT_TERMINAL_PROMPT = $previousPrompt
            }
            if ($null -eq $previousGcmInteractive) {
                Remove-Item Env:GCM_INTERACTIVE -ErrorAction SilentlyContinue
            } else {
                $env:GCM_INTERACTIVE = $previousGcmInteractive
            }
        }
    }

    Set-RdBootstrapGitHubTokenProcessCache $null
    return $null
}

function Get-RdBootstrapRelease {
    param(
        [Parameter(Mandatory)][string]$Repo,
        [string]$Version
    )
    $headers = @{
        Accept = 'application/vnd.github+json'
        'User-Agent' = 'rust-dependencies-bootstrap'
    }
    $token = Get-RdBootstrapGitHubToken
    if ($token) { $headers.Authorization = "Bearer $token" }

    $label = Resolve-RdBootstrapVersionLabel $Version
    $api = if ($label -eq 'latest') {
        "https://api.github.com/repos/$Repo/releases/latest"
    } else {
        "https://api.github.com/repos/$Repo/releases/tags/v$label"
    }
    $release = Invoke-RestMethod -Uri $api -Headers $headers
    $tag = [string]$release.tag_name
    if (-not $tag) { throw "GitHub release for '$Repo' did not include a tag_name." }
    $versionNumber = $tag.TrimStart('v', 'V')
    $zipName = "rust-dependencies-$versionNumber.zip"
    $zipAsset = @($release.assets | Where-Object name -eq $zipName) | Select-Object -First 1
    if (-not $zipAsset) { throw "Release '$tag' does not contain asset '$zipName'." }
    $expectedSha256 = ConvertFrom-RdBootstrapAssetDigest -Digest ([string]$zipAsset.digest) -AssetName $zipName
    [pscustomobject]@{
        Tag = $tag
        Version = $versionNumber
        ZipName = $zipName
        ZipUrl = [string]$zipAsset.browser_download_url
        ExpectedSha256 = $expectedSha256
    }
}

function ConvertFrom-RdBootstrapAssetDigest {
    param(
        [Parameter(Mandatory)][AllowEmptyString()][string]$Digest,
        [Parameter(Mandatory)][string]$AssetName
    )
    if ([string]::IsNullOrWhiteSpace($Digest)) {
        throw "Release asset '$AssetName' has no GitHub digest. Re-publish the release or use a newer GitHub API response."
    }
    if ($Digest -match '^(?i:sha256):(?<hash>[A-Fa-f0-9]{64})$') {
        return $Matches['hash'].ToLowerInvariant()
    }
    throw "Unsupported GitHub asset digest '$Digest' for '$AssetName'. Expected sha256:<64-hex>."
}

function Test-RdBootstrapToolkitRoot {
    param([Parameter(Mandatory)][string]$Path)
    $entrypoint = Join-Path $Path 'scripts' 'utility' 'Invoke-RustDependencies.ps1'
    return (Test-Path -LiteralPath $entrypoint -PathType Leaf)
}

function Get-RdBootstrapToolkitCacheRoot {
    if ($env:RD_TOOLKIT_CACHE) {
        return [IO.Path]::GetFullPath($env:RD_TOOLKIT_CACHE)
    }
    if ($IsWindows) {
        $base = if ($env:LOCALAPPDATA) { $env:LOCALAPPDATA } else { Join-Path $HOME 'AppData' 'Local' }
        return Join-Path $base 'rust-dependencies' 'toolkit'
    }
    $xdg = if ($env:XDG_CACHE_HOME) { $env:XDG_CACHE_HOME } else { Join-Path $HOME '.cache' }
    return Join-Path $xdg 'rust-dependencies' 'toolkit'
}

function Get-RdBootstrapLatestPointerPath {
    Join-Path (Get-RdBootstrapToolkitCacheRoot) 'latest'
}

function Read-RdBootstrapLatestPointer {
    $path = Get-RdBootstrapLatestPointerPath
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { return $null }
    $version = ([string](Get-Content -LiteralPath $path -Raw)).Trim()
    if ([string]::IsNullOrWhiteSpace($version)) { return $null }
    return $version
}

function Write-RdBootstrapLatestPointer {
    param([Parameter(Mandatory)][string]$Version)
    $cacheRoot = Get-RdBootstrapToolkitCacheRoot
    New-Item -ItemType Directory -Path $cacheRoot -Force | Out-Null
    Set-Content -LiteralPath (Get-RdBootstrapLatestPointerPath) -Value $Version.Trim() -NoNewline
}

function Test-RdBootstrapCachedToolkitVersion {
    param([Parameter(Mandatory)][string]$Version)
    $versionRoot = Join-Path (Get-RdBootstrapToolkitCacheRoot) $Version
    $marker = Join-Path $versionRoot 'scripts' 'utility' 'Invoke-RustDependencies.ps1'
    if (-not (Test-Path -LiteralPath $marker -PathType Leaf)) { return $null }
    return (Resolve-Path -LiteralPath $versionRoot).Path
}

function Get-RdBootstrapNewestCachedToolkitVersion {
    $cacheRoot = Get-RdBootstrapToolkitCacheRoot
    if (-not (Test-Path -LiteralPath $cacheRoot -PathType Container)) { return $null }
    $ranked = @(
        Get-ChildItem -LiteralPath $cacheRoot -Directory -ErrorAction SilentlyContinue |
            Where-Object { $null -ne (Test-RdBootstrapCachedToolkitVersion $_.Name) } |
            ForEach-Object {
                try {
                    [pscustomobject]@{ Name = $_.Name; Version = [version]$_.Name }
                } catch {
                    # Ignore non-semver cache folder names.
                }
            } |
            Sort-Object Version -Descending
    )
    if ($ranked.Count -eq 0) { return $null }
    return [string]$ranked[0].Name
}

function Install-RdBootstrapToolkit {
    param(
        [Parameter(Mandatory)][string]$ProjectRoot,
        [Parameter(Mandatory)][string]$Repo,
        [string]$Version,
        [string]$ToolkitRoot
    )
    if ($ToolkitRoot) {
        if (-not (Test-RdBootstrapToolkitRoot $ToolkitRoot)) {
            throw "ToolkitRoot '$ToolkitRoot' is missing scripts/utility/Invoke-RustDependencies.ps1."
        }
        return (Resolve-Path -LiteralPath $ToolkitRoot).Path
    }

    if (Test-RdBootstrapToolkitRoot $ProjectRoot) {
        Write-Host "Using local toolkit at '$ProjectRoot' (skipping download)."
        return (Resolve-Path -LiteralPath $ProjectRoot).Path
    }

    $label = Resolve-RdBootstrapVersionLabel $Version
    $forceRefresh = ($env:RD_TOOLKIT_REFRESH -eq '1')
    $cacheRoot = Get-RdBootstrapToolkitCacheRoot

    # Prefer an already-installed toolkit without calling GitHub. Consumer projects
    # previously hit releases/latest on every run, which pulled GCM login UI whenever
    # anonymous API quota was exhausted and no durable token was configured.
    if (-not $forceRefresh) {
        if ($label -eq 'latest') {
            $pinned = Read-RdBootstrapLatestPointer
            if (-not $pinned) {
                $pinned = Get-RdBootstrapNewestCachedToolkitVersion
                if ($pinned) { Write-RdBootstrapLatestPointer $pinned }
            }
            if ($pinned) {
                $cachedLatest = Test-RdBootstrapCachedToolkitVersion $pinned
                if ($cachedLatest) {
                    Write-Host "Using cached toolkit v$pinned at '$cachedLatest' (set RD_TOOLKIT_REFRESH=1 to check GitHub for a newer release)."
                    return $cachedLatest
                }
            }
        } else {
            $cachedPinned = Test-RdBootstrapCachedToolkitVersion $label
            if ($cachedPinned) {
                Write-Host "Using cached toolkit v$label at '$cachedPinned'."
                return $cachedPinned
            }
        }
    }

    $release = Get-RdBootstrapRelease -Repo $Repo -Version $Version
    $versionRoot = Join-Path $cacheRoot $release.Version
    $marker = Join-Path $versionRoot 'scripts' 'utility' 'Invoke-RustDependencies.ps1'
    if (Test-Path -LiteralPath $marker -PathType Leaf) {
        if ($label -eq 'latest') { Write-RdBootstrapLatestPointer $release.Version }
        Write-Host "Using cached toolkit $($release.Tag) at '$versionRoot'."
        return (Resolve-Path -LiteralPath $versionRoot).Path
    }

    New-Item -ItemType Directory -Path $cacheRoot -Force | Out-Null
    $temp = Join-Path ([IO.Path]::GetTempPath()) "rd-bootstrap-$([guid]::NewGuid().ToString('N'))"
    New-Item -ItemType Directory -Path $temp -Force | Out-Null
    try {
        $zipPath = Join-Path $temp $release.ZipName
        Write-Host "Downloading $($release.ZipName) from $Repo $($release.Tag)..."
        Invoke-WebRequest -Uri $release.ZipUrl -OutFile $zipPath -UseBasicParsing
        $actual = (Get-FileHash -LiteralPath $zipPath -Algorithm SHA256).Hash.ToLowerInvariant()
        if ($actual -ne $release.ExpectedSha256) {
            throw "SHA-256 mismatch for '$($release.ZipName)'. Expected $($release.ExpectedSha256) but got $actual."
        }
        $extract = Join-Path $temp 'extract'
        Expand-Archive -LiteralPath $zipPath -DestinationPath $extract -Force
        $prefixed = Join-Path $extract "rust-dependencies-$($release.Version)"
        $sourceRoot = if (Test-RdBootstrapToolkitRoot $prefixed) {
            $prefixed
        } elseif (Test-RdBootstrapToolkitRoot $extract) {
            $extract
        } else {
            $found = Get-ChildItem -LiteralPath $extract -Directory -Recurse -ErrorAction SilentlyContinue |
                Where-Object { Test-RdBootstrapToolkitRoot $_.FullName } |
                Select-Object -First 1
            if (-not $found) {
                throw "Downloaded archive did not contain a toolkit root with scripts/utility/Invoke-RustDependencies.ps1."
            }
            $found.FullName
        }
        if (Test-Path -LiteralPath $versionRoot) {
            Remove-Item -LiteralPath $versionRoot -Recurse -Force
        }
        New-Item -ItemType Directory -Path (Split-Path -Parent $versionRoot) -Force | Out-Null
        Copy-Item -LiteralPath $sourceRoot -Destination $versionRoot -Recurse -Force
        if (-not (Test-RdBootstrapToolkitRoot $versionRoot)) {
            throw "Failed to install toolkit into '$versionRoot'."
        }
        if ($label -eq 'latest') { Write-RdBootstrapLatestPointer $release.Version }
        Write-Host "Installed toolkit $($release.Tag) at '$versionRoot'."
        return (Resolve-Path -LiteralPath $versionRoot).Path
    }
    finally {
        if (Test-Path -LiteralPath $temp) {
            Remove-Item -LiteralPath $temp -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
}

function Update-RdBootstrapEntrypoint {
    param(
        [Parameter(Mandatory)][string]$ProjectRoot,
        [Parameter(Mandatory)][string]$ToolkitRoot
    )
    $source = Join-Path $ToolkitRoot 'download-dependencies.ps1'
    $target = Join-Path $ProjectRoot 'download-dependencies.ps1'
    if (-not (Test-Path -LiteralPath $source -PathType Leaf)) {
        return $false
    }
    if (-not (Test-Path -LiteralPath $target -PathType Leaf)) {
        Copy-Item -LiteralPath $source -Destination $target -Force
        Write-Host "Installed project bootstrap at '$target' from toolkit $($ToolkitRoot)."
        return $true
    }
    $sourceHash = (Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash
    $targetHash = (Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash
    if ($sourceHash -eq $targetHash) {
        return $false
    }
    Copy-Item -LiteralPath $source -Destination $target -Force
    Write-Host "Updated project bootstrap at '$target' from toolkit $($ToolkitRoot)."
    return $true
}

function Get-RdBootstrapRelaunchArgumentList {
    param(
        [Parameter(Mandatory)][string]$Entrypoint,
        [string]$Version,
        [string]$Script,
        [string]$ServerPlatform,
        [switch]$Publicize,
        [switch]$PublicizeBound,
        [switch]$WhatIf,
        [string]$Repo,
        [string]$ProjectRoot,
        [string]$ToolkitRoot
    )
    $args = [Collections.Generic.List[string]]::new()
    $args.AddRange([string[]]@('-NoProfile', '-File', $Entrypoint))
    if (-not [string]::IsNullOrWhiteSpace($Version)) { $args.AddRange([string[]]@('-Version', $Version)) }
    if (-not [string]::IsNullOrWhiteSpace($Script)) { $args.AddRange([string[]]@('-Script', $Script)) }
    if (-not [string]::IsNullOrWhiteSpace($ServerPlatform)) { $args.AddRange([string[]]@('-ServerPlatform', $ServerPlatform)) }
    if ($PublicizeBound) {
        if ($Publicize) { $args.Add('-Publicize') }
        else { $args.Add('-Publicize:$false') }
    }
    if ($WhatIf) { $args.Add('-WhatIf') }
    if (-not [string]::IsNullOrWhiteSpace($Repo) -and $Repo -ne 'ilovepatatos-rust/rust-dependencies') {
        $args.AddRange([string[]]@('-Repo', $Repo))
    }
    if (-not [string]::IsNullOrWhiteSpace($ProjectRoot)) { $args.AddRange([string[]]@('-ProjectRoot', $ProjectRoot)) }
    if (-not [string]::IsNullOrWhiteSpace($ToolkitRoot)) { $args.AddRange([string[]]@('-ToolkitRoot', $ToolkitRoot)) }
    return , $args.ToArray()
}

function Get-RdBootstrapManifestPath {
    param(
        [Parameter(Mandatory)][string]$ProjectRoot,
        [Parameter(Mandatory)][string]$ToolkitRoot,
        [Parameter(Mandatory)][ValidateSet('Production', 'Staging')][string]$Channel
    )
    $productionPath = Join-Path $ProjectRoot 'rust-dependencies.json'
    $stagingPath = Join-Path $ProjectRoot 'rust-staging-dependencies.json'
    $selected = if ($Channel -eq 'Staging' -and (Test-Path -LiteralPath $stagingPath -PathType Leaf)) {
        $stagingPath
    } else {
        $productionPath
    }
    if ($selected -eq $productionPath -and -not (Test-Path -LiteralPath $productionPath -PathType Leaf)) {
        $example = Join-Path $ToolkitRoot 'examples' 'rust-dependencies.example.json'
        if (-not (Test-Path -LiteralPath $example -PathType Leaf)) {
            throw "Project manifest '$productionPath' is missing and toolkit example '$example' was not found."
        }
        Copy-Item -LiteralPath $example -Destination $productionPath
        Write-Host "Created project manifest at '$productionPath' from the toolkit example."
    }
    if (-not (Test-Path -LiteralPath $selected -PathType Leaf)) {
        throw "Resolved project manifest '$selected' was not found."
    }
    return (Resolve-Path -LiteralPath $selected).Path
}

function Resolve-RdBootstrapMenuSelection {
    param(
        [string]$Script,
        [scriptblock]$ReadHostCommand
    )
    $menu = @(Get-RdBootstrapMenuEntries)
    if (-not [string]::IsNullOrWhiteSpace($Script)) {
        $normalized = $Script.Trim() -replace '\.ps1$', ''
        $match = @($menu | Where-Object { $_.Name -eq $normalized })
        if ($match.Count -ne 1) {
            throw "Unknown -Script '$Script'. Expected one of: $($menu.Name -join ', ')."
        }
        return $match[0]
    }

    Write-Host ''
    Write-Host 'Select a dependencies download script:'
    foreach ($entry in $menu) {
        Write-Host ("{0}. {1}" -f $entry.Number, $entry.Name)
    }
    if (-not $ReadHostCommand) {
        $ReadHostCommand = { param($prompt) Read-Host $prompt }
    }
    $choice = & $ReadHostCommand 'Enter choice (1-9)'
    $selected = @($menu | Where-Object { $_.Number.ToString() -eq "$choice" })
    if ($selected.Count -ne 1) {
        $choice = & $ReadHostCommand 'Invalid choice. Enter choice (1-9)'
        $selected = @($menu | Where-Object { $_.Number.ToString() -eq "$choice" })
    }
    if ($selected.Count -ne 1) {
        throw "Invalid menu choice '$choice'. Expected a number from 1 to 9."
    }
    return $selected[0]
}

function ConvertFrom-RdBootstrapYesNo {
    param([string]$Value)
    switch -Regex ($Value.Trim()) {
        '^(?i:y|yes)$' { return $true }
        '^(?i:n|no)$' { return $false }
        default { return $null }
    }
}

function Resolve-RdBootstrapPublicize {
    param(
        [bool]$PublicizeBound,
        [bool]$Publicize,
        [scriptblock]$ReadHostCommand
    )
    if ($PublicizeBound) { return $Publicize }

    Write-Host ''
    Write-Host 'Publicize dependencies after download?'
    if (-not $ReadHostCommand) {
        $ReadHostCommand = { param($prompt) Read-Host $prompt }
    }
    $answer = ConvertFrom-RdBootstrapYesNo (& $ReadHostCommand 'Enter y/n')
    if ($null -eq $answer) {
        $answer = ConvertFrom-RdBootstrapYesNo (& $ReadHostCommand 'Invalid choice. Enter y/n')
    }
    if ($null -eq $answer) {
        throw "Invalid publicize choice. Expected y/yes or n/no."
    }
    return $answer
}

function ConvertFrom-RdBootstrapServerPlatform {
    param([string]$Value)
    $trimmed = "$Value".Trim()
    switch -Regex ($trimmed) {
        '^(?i:windows|1)$' { return 'Windows' }
        '^(?i:linux|2)$' { return 'Linux' }
        '^(?i:all|both|3)$' { return 'All' }
        default { return $null }
    }
}

function Resolve-RdBootstrapServerPlatform {
    param(
        [string]$ServerPlatform,
        [scriptblock]$ReadHostCommand
    )
    if (-not [string]::IsNullOrWhiteSpace($ServerPlatform)) {
        $resolved = ConvertFrom-RdBootstrapServerPlatform $ServerPlatform
        if (-not $resolved) {
            throw "Invalid -ServerPlatform '$ServerPlatform'. Expected Windows, Linux, or All."
        }
        return $resolved
    }

    Write-Host ''
    Write-Host 'Select server platform:'
    Write-Host '1. Windows'
    Write-Host '2. Linux'
    Write-Host '3. All (Windows and Linux)'
    if (-not $ReadHostCommand) {
        $ReadHostCommand = { param($prompt) Read-Host $prompt }
    }
    $choice = ConvertFrom-RdBootstrapServerPlatform (& $ReadHostCommand 'Enter choice (1-3 or Windows/Linux/All)')
    if (-not $choice) {
        $choice = ConvertFrom-RdBootstrapServerPlatform (& $ReadHostCommand 'Invalid choice. Enter choice (1-3 or Windows/Linux/All)')
    }
    if (-not $choice) {
        throw "Invalid server platform choice. Expected Windows/Linux/All or 1/2/3."
    }
    return $choice
}

function Invoke-RdBootstrap {
    [CmdletBinding()]
    param(
        [string]$Version,
        [string]$Script,
        [string]$ServerPlatform,
        [switch]$Publicize,
        [switch]$PublicizeBound,
        [switch]$WhatIf,
        [string]$Repo = 'ilovepatatos-rust/rust-dependencies',
        [string]$ProjectRoot,
        [string]$ToolkitRoot,
        [scriptblock]$ReadHostCommand
    )
    if (-not $ProjectRoot) {
        if ($PSScriptRoot) { $ProjectRoot = $PSScriptRoot }
        else { $ProjectRoot = (Get-Location).Path }
    }
    $ProjectRoot = (Resolve-Path -LiteralPath $ProjectRoot).Path
    $installedRoot = Install-RdBootstrapToolkit -ProjectRoot $ProjectRoot -Repo $Repo -Version $Version -ToolkitRoot $ToolkitRoot

    # Keep the consumer project's bootstrap in sync with the resolved toolkit, then
    # re-launch once so the updated script (menus, auth, etc.) actually runs.
    $skipSelfUpdate = ($env:RD_BOOTSTRAP_REEXEC -eq '1') -or (Test-RdBootstrapToolkitRoot $ProjectRoot)
    if (-not $skipSelfUpdate -and (Update-RdBootstrapEntrypoint -ProjectRoot $ProjectRoot -ToolkitRoot $installedRoot)) {
        $entry = Join-Path $ProjectRoot 'download-dependencies.ps1'
        if ($env:RD_BOOTSTRAP_UNITTEST -eq '1') {
            Write-Host "Bootstrap entrypoint updated at '$entry' (unit test mode; not re-executing)."
        } else {
            $pwsh = (Get-Process -Id $PID).Path
            $relaunchArgs = Get-RdBootstrapRelaunchArgumentList `
                -Entrypoint $entry `
                -Version $Version `
                -Script $Script `
                -ServerPlatform $ServerPlatform `
                -Publicize:$Publicize `
                -PublicizeBound:$PublicizeBound `
                -WhatIf:$WhatIf `
                -Repo $Repo `
                -ProjectRoot $ProjectRoot `
                -ToolkitRoot $ToolkitRoot
            return [pscustomobject]@{
                Action = 'Reexec'
                CommandPath = $pwsh
                Arguments = $relaunchArgs
            }
        }
    }

    $selection = Resolve-RdBootstrapMenuSelection -Script $Script -ReadHostCommand $ReadHostCommand
    $usePublicize = Resolve-RdBootstrapPublicize -PublicizeBound:$PublicizeBound -Publicize:([bool]$Publicize) -ReadHostCommand $ReadHostCommand
    $resolvedPlatform = Resolve-RdBootstrapServerPlatform -ServerPlatform $ServerPlatform -ReadHostCommand $ReadHostCommand
    $invoke = @{
        ServerPlatform = $resolvedPlatform
    }
    if ($usePublicize) { $invoke.Publicize = $true }
    if ($WhatIf) { $invoke.WhatIf = $true }

    if (-not $selection.Channel) {
        # Hand off to scripts/all.ps1 with explicit project manifests (parallel main+staging).
        $prodManifest = Get-RdBootstrapManifestPath -ProjectRoot $ProjectRoot -ToolkitRoot $installedRoot -Channel Production
        $stagingManifest = Get-RdBootstrapManifestPath -ProjectRoot $ProjectRoot -ToolkitRoot $installedRoot -Channel Staging
        $scriptPath = Join-Path $installedRoot 'scripts' 'all.ps1'
        if (-not (Test-Path -LiteralPath $scriptPath -PathType Leaf)) {
            throw "Toolkit script '$scriptPath' was not found."
        }
        Write-Host "Running all (production and staging in parallel) under '$ProjectRoot'..."
        Write-Host "  production manifest: $prodManifest"
        Write-Host "  staging manifest:    $stagingManifest"
        $null = & $scriptPath @invoke -ManifestPath $prodManifest -StagingManifestPath $stagingManifest
        return
    }

    $scriptPath = Join-Path $installedRoot 'scripts' "$($selection.Name).ps1"
    if (-not (Test-Path -LiteralPath $scriptPath -PathType Leaf)) {
        throw "Toolkit script '$scriptPath' was not found."
    }
    $manifestPath = Get-RdBootstrapManifestPath -ProjectRoot $ProjectRoot -ToolkitRoot $installedRoot -Channel $selection.Channel
    $invoke.ManifestPath = $manifestPath
    Write-Host "Running $($selection.Name) with manifest '$manifestPath'..."
    $null = & $scriptPath @invoke
}

function Wait-RdBootstrapKeyPress {
    if ($env:RD_BOOTSTRAP_UNITTEST -eq '1') { return }
    if ($env:CI -or $env:GITHUB_ACTIONS) { return }
    if (-not [Environment]::UserInteractive) { return }
    Write-Host ''
    Write-Host 'Press Enter to exit...'
    try {
        [void][Console]::ReadLine()
    } catch {
        Read-Host 'Press Enter to exit'
    }
}

if ($env:RD_BOOTSTRAP_UNITTEST -ne '1') {
    $script:RdBootstrapFailed = $false
    $script:RdBootstrapSkipKeyPress = $false
    try {
        $bootstrapResult = Invoke-RdBootstrap `
            -Version $Version `
            -Script $Script `
            -ServerPlatform $ServerPlatform `
            -Publicize:$Publicize `
            -PublicizeBound:($PSBoundParameters.ContainsKey('Publicize')) `
            -WhatIf:$WhatIf `
            -Repo $Repo `
            -ProjectRoot $ProjectRoot `
            -ToolkitRoot $ToolkitRoot
        if ($bootstrapResult -and $bootstrapResult.Action -eq 'Reexec') {
            $script:RdBootstrapSkipKeyPress = $true
            Write-Host "Re-running updated bootstrap..."
            $previousReexec = $env:RD_BOOTSTRAP_REEXEC
            $env:RD_BOOTSTRAP_REEXEC = '1'
            try {
                & $bootstrapResult.CommandPath @($bootstrapResult.Arguments)
                if ($LASTEXITCODE -ne 0) { $script:RdBootstrapFailed = $true }
            } finally {
                if ($null -eq $previousReexec) {
                    Remove-Item Env:RD_BOOTSTRAP_REEXEC -ErrorAction SilentlyContinue
                } else {
                    $env:RD_BOOTSTRAP_REEXEC = $previousReexec
                }
            }
        }
    } catch {
        $script:RdBootstrapFailed = $true
        Write-Host ''
        Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.ScriptStackTrace) {
            Write-Host $_.ScriptStackTrace -ForegroundColor DarkRed
        }
    } finally {
        if (-not $script:RdBootstrapSkipKeyPress) {
            Wait-RdBootstrapKeyPress
        }
    }
    if ($script:RdBootstrapFailed) { exit 1 }
}
