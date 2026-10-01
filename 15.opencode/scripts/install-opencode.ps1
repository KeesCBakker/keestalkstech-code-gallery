& {
  function Update-Path {
    $machinePath = [Environment]::GetEnvironmentVariable("Path", "Machine")
    $userPath = [Environment]::GetEnvironmentVariable("Path", "User")
    $env:Path = "$machinePath;$userPath"
  }

  # Install OpenCode and Coreutils.
  winget install --id SST.opencode
  winget install --id Microsoft.Coreutils

  # Verify the installed OpenCode executable.
  Write-Host "opencode: $(opencode --version)"
}
