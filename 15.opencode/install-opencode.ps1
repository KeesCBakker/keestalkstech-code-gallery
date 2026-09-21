& {
  function Update-Path {
    $machinePath = [Environment]::GetEnvironmentVariable("Path", "Machine")
    $userPath = [Environment]::GetEnvironmentVariable("Path", "User")
    $env:Path = "$machinePath;$userPath"
  }

  # Install OpenCode, NVM for Windows, and Coreutils.
  winget install --id SST.opencode
  winget install --id CoreyButler.NVMforWindows
  winget install --id Microsoft.Coreutils

  # Load tools installed by WinGet into this PowerShell session.
  Update-Path

  # Allow npm and npx PowerShell wrappers for the current user.
  Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

  # Install and activate the latest LTS version of Node.js.
  nvm install lts
  nvm use lts

  # nvm-windows updates its symlink asynchronously.
  Start-Sleep -Seconds 1

  # Load the updated Node.js path and verify the installed tools.
  Update-Path
  Write-Host "opencode: $(opencode --version)"
  Write-Host "node: $(node --version)"
  Write-Host "npm: $(npm --version)"
  Write-Host "npx: $(npx --version)"
}
