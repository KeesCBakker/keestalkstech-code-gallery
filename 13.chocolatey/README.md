# Chocolatey
With the <a href="https://community.chocolatey.org/">Chocolatey Package Manager</a> for Windows, it is super easy to install software from the command-line. This makes your installs scriptable and thus repeatable. In this article, I'll show you how to render installation instructions from a machine and how to use the Windows Task Scheduler to update your packages regularly.

- <a href="13.chocolatey">13.chocolatey</a>
- <a href="https://keestalkstech.com/notes-on-chocolatey/">Notes on Chocolatey</a>

## Checkout only this project

Do the following:

```sh
git clone --no-checkout https://github.com/KeesCBakker/keestalkstech-code-gallery.git
cd keestalkstech-code-gallery
git sparse-checkout init
git sparse-checkout set --no-cone 13.chocolatey
git checkout main
cd 13.chocolatey
ls
```
