# Bypass Company Wallpaper as Local Admin

Looks like my organization wants to manage my background picture. But I'm a local admin,
so let's see if we can still change that background picture. When I go
to Setting > Personalization > Background, I see the message:
"Some of these settings are managed by your organization." If you are an admin,
you might be able to (temporarily) override the wallpaper, by editing your registry and
triggering a refresh of the wallpaper. If you can, you can automate it using PowerShell.

The code is a companion of the blog [How to bypass company wallpaper as local admin on Windows 11](https://keestalkstech.com/how-to-bypass-company-background-image-as-local-admin-on-windows-11/).

## Prerequisites

- Windows 10 or 11
- Local administrator rights on the machine
- PowerShell 5.1+ (ships with Windows)

## Files

| File | Description |
|------|-------------|
| `ChangeWallpaper.ps1` | Main script: copies a wallpaper image, updates the registry, and refreshes the desktop |
| `SetupTask.ps1` | Creates a scheduled task that runs `ChangeWallpaper.ps1` at user logon (requires admin) |
| `Init.ps1` | One-click setup: copies `ChangeWallpaper.ps1` to MyPictures, runs `SetupTask.ps1`, then executes the change script |

## Quick start

Run from an elevated (admin) PowerShell prompt:

```powershell
.\Init.ps1
```

This will:
1. Copy `ChangeWallpaper.ps1` to your `MyPictures` folder
2. Register a scheduled task named `ChangeWallpaperOnLogin` (triggers 30s after logon)
3. Execute `ChangeWallpaper.ps1` immediately to apply the wallpaper

> Run `Init.ps1` from a non-admin prompt — `SetupTask.ps1` will exit with an error.

## Configuration

Edit the variables at the top of `ChangeWallpaper.ps1`:

| Variable | Default | Description |
|----------|---------|-------------|
| `$PicturesFolder` | `MyPictures` | Destination folder for the wallpaper copy |
| `$BackgroundImagesPath` | `$PicturesFolder\Windows Spotlight Images` | Source folder with `.jpg` images |
| `$StaticBackgroundImagePath` | `$null` | Use a fixed image instead of the latest from Spotlight |
| `$EnablePauseBeforeAdminPrompt` | `$true` | Show a `Pause` prompt before the UAC elevation dialog |
| `$CountDownBeforeContinueSeconds` | `5` | How long the success/status message stays on screen |

### Wallpaper selection logic

- If `$StaticBackgroundImagePath` is set, that image is used
- Otherwise the most recent `.jpg` from `$BackgroundImagesPath` is picked
- The selected image is copied to `$PicturesFolder\wallpaper.jpg` so registry points to a stable path
- Changes are detected by comparing the current registry values — no unnecessary updates

## How it works

1. The script copies the Spotlight image to `wallpaper.jpg` (if it changed)
2. It sets three registry keys under `HKCU:\Software\Microsoft\Windows\CurrentVersion\Policies\System`:
   - `Wallpaper` → path to `wallpaper.jpg`
   - `WallpaperStyle` → `4` (stretched)
   - `TileWallpaper` → `0` (no tiling)
3. It calls `SystemParametersInfo` via P/Invoke to refresh the desktop immediately

> These registry keys override organization policy for the current user until the next policy refresh or reboot.

## Reverting

### Remove the scheduled task

```powershell
Unregister-ScheduledTask -TaskName "ChangeWallpaperOnLogin" -Confirm:$false
```

### Reset wallpaper to organization policy

```powershell
Remove-ItemProperty -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Policies\System" -Name Wallpaper
Remove-ItemProperty -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Policies\System" -Name WallpaperStyle
Remove-ItemProperty -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Policies\System" -Name TileWallpaper
```

Then restart or run `ChangeWallpaper.ps1` again to trigger a refresh without a custom wallpaper path.

## Checkout only this project

```powershell
git clone --no-checkout https://github.com/KeesCBakker/keestalkstech-code-gallery.git
cd keestalkstech-code-gallery
git sparse-checkout init
git sparse-checkout set --no-cone 08.bypass-company-wallpaper
git checkout main
cd 08.bypass-company-wallpaper
dir
```
