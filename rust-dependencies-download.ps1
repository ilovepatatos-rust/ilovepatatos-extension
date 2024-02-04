$resourcesDir = Join-Path -Path $PSScriptRoot -ChildPath "src/dependencies"

# Create the directory if it doesn't exist
if (-not(Test-Path -Path $resourcesDir))
{
    New-Item -Path $resourcesDir -ItemType Directory
}

# Create a temporary folder
$tmpDir = New-Item -ItemType Directory -Force -Path "$env:TEMP\download-dependencies"

# Temporary folders for DepotDownloader and Rust DLLs
$depotDir = Join-Path -Path $tmpDir -ChildPath "DepotDownloader"
$rustDir = Join-Path -Path $depotDir -ChildPath "RustDLLs"

# Create a temporary folder for DepotDownloader and Rust DLLs
New-Item -ItemType Directory -Force -Path $depotDir
New-Item -ItemType Directory -Force -Path $rustDir

# Download and extract DepotDownloader (presumably for Windows)
Invoke-WebRequest -Uri "https://github.com/SteamRE/DepotDownloader/releases/latest/download/DepotDownloader-windows-x64.zip" -OutFile "$depotDir\DepotDownloader.zip"
Expand-Archive -Path "$depotDir\DepotDownloader.zip" -DestinationPath $depotDir -Force
Remove-Item -Path "$depotDir\DepotDownloader.zip"

# Create a file named filelist.txt for downloading .dll files
$fileListPath = Join-Path -Path $rustDir -ChildPath "filelist.txt"
@("regex:RustDedicated_Data/Managed/.*\.dll") | Set-Content -Path $fileListPath

# Download .dll files using DepotDownloader
$depotArgs = "-app 258550 -depot 258551 -filelist $fileListPath -dir $rustDir"
Start-Process -FilePath "$depotDir\DepotDownloader.exe" -ArgumentList $depotArgs -NoNewWindow -Wait

# Move .dll files to the current directory
Move-Item -Path "$rustDir\RustDedicated_Data\Managed\*.dll" -Destination $resourcesDir -Force

# Download and extract Oxide (presumably for Windows)
$oxideDir = Join-Path -Path $tmpDir -ChildPath "Oxide"
New-Item -ItemType Directory -Force -Path $oxideDir
Invoke-WebRequest -Uri "https://github.com/OxideMod/Oxide.Rust/releases/latest/download/Oxide.Rust.zip" -OutFile "$oxideDir\Oxide.Rust.zip"
Expand-Archive -Path "$oxideDir\Oxide.Rust.zip" -DestinationPath $oxideDir -Force
Remove-Item -Path "$oxideDir\Oxide.Rust.zip"

# Move .dll files from Oxide to the current directory
Move-Item -Path "$oxideDir\RustDedicated_Data\Managed\*.dll" -Destination $resourcesDir -Force

# Delete the temporary folder
Remove-Item -Path $tmpDir -Force -Recurse

# Delete dll breaking automated builds using git actions
$filesToDelete = @(
"Facepunch.Steamworks.Win64.dll",
"UnityEngine.ARModule.dll",
"UnityEngine.NVIDIAModule.dll"
)

foreach ($filename in $filesToDelete)
{
    $filePath = Join-Path -Path $resourcesDir -ChildPath $filename
    if (Test-Path -Path $filePath)
    {
        Remove-Item -Path $filePath -Force
    }
}