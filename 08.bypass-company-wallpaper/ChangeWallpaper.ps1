# Config options:
$PicturesFolder = [Environment]::GetFolderPath('MyPictures')
$BackgroundImagesPath = "$PicturesFolder\Windows Spotlight Images"
$StaticBackgroundImagePath = $null
$EnablePauseBeforeAdminPrompt = $true
$CountDownBeforeContinueSeconds = 5

# Vars
$WallpaperPath = "$PicturesFolder\wallpaper2.jpg"
$WallpaperTxtPath = "$PicturesFolder\wallpaper.txt"
$WallpaperStyle = 4
$TileWallpaper = 0
$Restart = $false
$ImagePath = ""
$RegistryRootKey       = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Policies\System"
$CurrentWallpaper      = (Get-ItemProperty -Path $RegistryRootKey -Name Wallpaper).Wallpaper
$CurrentWallpaperStyle = (Get-ItemProperty -Path $RegistryRootKey -Name WallpaperStyle).WallpaperStyle
$CurrentTileWallpaper  = (Get-ItemProperty -Path $RegistryRootKey -Name TileWallpaper).TileWallpaper

$ShouldChange = $CurrentWallpaper -ne $WallpaperPath -or `
                $CurrentWallpaperStyle -ne $WallpaperStyle -or `
                $CurrentTileWallpaper -ne $TileWallpaper

$IsAdmin = $null -ne ([Security.Principal.WindowsIdentity]::GetCurrent().Groups | Where-Object { $_.Value -eq 'S-1-5-32-544' })
$ErrorActionPreference = "Stop" # Fail on the first error

# Step 1: Determine the image path
if ($StaticBackgroundImagePath) {
    $ImagePath = $StaticBackgroundImagePath
} else {
    $LatestImage = Get-ChildItem -Path $BackgroundImagesPath -Filter *.jpg | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    $ImagePath = $LatestImage.FullName
}

if (-not $ImagePath) {
    Write-Output "No wallpaper found. Please check \$StaticBackgroundImagePath or \$BackgroundImagesPath."
    timeout /t $CountDownBeforeContinueSeconds
    return
}

# Step 1.1: Verify and copy the latest image to wallpaper.jpg if needed
if (-not (Test-Path $WallpaperTxtPath) -or (Get-Content $WallpaperTxtPath -ErrorAction SilentlyContinue) -ne $ImagePath) {
    Copy-Item -Path $ImagePath -Destination $WallpaperPath -Force
    Set-Content -Path $WallpaperTxtPath -Value $ImagePath
    Write-Output "Latest image copied to $WallpaperPath"
    $Restart = $true
}

# Step 1.2: Check if the registry is set to wallpaper.jpg (requires elevation if needed)
if (-not $IsAdmin -and $ShouldChange) {
    Write-Output "Script requires elevation to update the registry. Restarting with elevated privileges..."
    if ($EnablePauseBeforeAdminPrompt) { Pause }
    Start-Process powershell -ArgumentList "-NoProfile -ExecutionPolicy Bypass -Command `"$PSCommandPath" -Verb RunAs
    return
}

if ($ShouldChange) {
    Set-ItemProperty -Path $RegistryRootKey -Name Wallpaper -Value $WallpaperPath -ErrorAction SilentlyContinue
    Set-ItemProperty -Path $RegistryRootKey -Name WallpaperStyle -Value 4
    Set-ItemProperty -Path $RegistryRootKey -Name TileWallpaper -Value 0
    Write-Output "Registry updated to use $WallpaperPath as the wallpaper."
    $Restart = $true
}

# Step 2: Reload the wallpaper
if (-not $Restart) {
    Write-Output "No changes needed; wallpaper is already set correctly."
    timeout /t $CountDownBeforeContinueSeconds
    return
}

try {
    Add-Type -TypeDefinition @"
    using System;
    using System.Runtime.InteropServices;

    public class Wallpaper {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);
    }
"@
}
catch {
    # only on the first time the script runs
    # the type can be added
 }

$SPI_SETDESKWALLPAPER = 0x0014
$SPIF_UPDATEINIFILE = 0x01
$SPIF_SENDCHANGE = 0x02

$Result = [Wallpaper]::SystemParametersInfo($SPI_SETDESKWALLPAPER, 0, $null, $SPIF_UPDATEINIFILE -bor $SPIF_SENDCHANGE)


Write-Output "Wallpaper changed."
timeout /t $CountDownBeforeContinueSeconds
