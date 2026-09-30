# WinGet

Use the Windows Package Manager to generate installation instructions, install a package list, run post-installation steps, and schedule package upgrades.

- Read more on the blog: <a href="https://keestalkstech.com/notes-on-winget/">Notes on winget</a>

## Scripts

- `list.ps1` exports recognized packages and prints install commands, retaining each package source.
- `packages.ps1` installs the curated package list, with Microsoft Store apps in a separate section.
- `upgrade-task.ps1` writes the upgrade command to the current user's local app data and registers a weekly Task Scheduler task. It requests silent, non-interactive operation where supported.
- `post-install.ps1` installs Node.js with NVM and npm-check-updates.

## Checkout only this project

```sh
git clone --no-checkout https://github.com/KeesCBakker/keestalkstech-code-gallery.git
cd keestalkstech-code-gallery
git sparse-checkout init
git sparse-checkout set --no-cone 18.winget
git checkout main
cd 18.winget
ls
```
