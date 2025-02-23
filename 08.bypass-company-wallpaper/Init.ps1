# Enable strict error handling to fail on the first error
$ErrorActionPreference = "Stop"

# Define the paths
$PictureDir = [Environment]::GetFolderPath("MyPictures")

# Copy ChangeWallpaper.ps1 to the Pictures directory
Write-Host "Copying change script to $PictureDir..."
Copy-Item -Path .\ChangeWallpaper.ps1 -Destination $PictureDir -Force

# Execute SetupTask.ps1
Write-Host "Executing task setup..."
& .\SetupTask.ps1

# Execute ChangeWallpaper.ps1 from the Pictures directory
Write-Host "Executing $ChangeWallpaperPictureScript..."
pushd $PictureDir
& .\ChangeWallpaper.ps1
popd
