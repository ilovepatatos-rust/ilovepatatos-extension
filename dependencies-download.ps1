$resourcesDir = Join-Path -Path $PSScriptRoot -ChildPath "src/dependencies"

# Create the directory if it doesn't exist
if (-not(Test-Path -Path $resourcesDir))
{
    New-Item -Path $resourcesDir -ItemType Directory
}

# Clear output directory
if (Test-Path -Path $resourcesDir)
{
    Get-ChildItem -Path $resourcesDir | Remove-Item -Force -Recurse
}

$files = @(
"rust-dependencies-download.ps1",
"public-dependencies-download.ps1"
)

# Loop through each file and execute it
foreach ($file in $files)
{
    # Construct the full path to the script file
    $scriptPath = Join-Path -Path $PSScriptRoot -ChildPath $file

    if (Test-Path $scriptPath)
    {
        Write-Host "Executing script: $file"
        & $scriptPath
    }
    else
    {
        Write-Host "Script file not found: $file"
    }
}