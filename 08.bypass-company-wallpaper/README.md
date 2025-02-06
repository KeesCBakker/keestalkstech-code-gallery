# Bypass Company Wallpaper as Local Admin

Looks like my organization wants to manage my background picture. But I'm a local admin, 
so let's see if we can still change that background picture. When I go 
to Setting > Personalization > Background, I see the message: 
"Some of these settings are managed by your organization." If you are an admin, 
you might be able to (temporarily) override the wallpaper, by editing your registry and 
triggering a refresh of the wallpaper. If you can, you can automate it using PowerShell.

The code is a companion of the blog <a href="https://keestalkstech.com/2024/12/how-to-bypass-company-background-image-as-local-admin-on-windows-11/">How to bypass company wallpaper as local admin on Windows 11</a>.

## Command to execute

Execute `./Init.ps1` to set things up. It will install the `./ChangeWallpaper.ps1` and
setup a refresh task.

## Checkout only this project

Do the following:

```sh
git clone --no-checkout https://github.com/KeesCBakker/keestalkstech-dotnet-gallery.git
cd keestalkstech-dotnet-gallery
git sparse-checkout init
git sparse-checkout set --no-cone 8.bypass-company-wallpaper
git checkout main
```