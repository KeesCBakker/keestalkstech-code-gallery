& {
  function Update-Path {
    $machinePath = [Environment]::GetEnvironmentVariable("Path", "Machine")
    $userPath = [Environment]::GetEnvironmentVariable("Path", "User")
    $env:Path = "$machinePath;$userPath"
  }

  # Install OpenCode and Coreutils.
  winget install --id SST.opencode
  winget install --id Microsoft.Coreutils

  # Load tools installed by WinGet into this PowerShell session.
  Update-Path

  # Verify the tools used by the installer and merge scripts.
  Update-Path
  Write-Host "opencode: $(opencode --version)"
  Write-Host "bun: $(bun --version)"
}
