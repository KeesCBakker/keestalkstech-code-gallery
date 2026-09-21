$ErrorActionPreference = "Stop"

function Write-Status {
  param(
    [string] $Message,
    [ConsoleColor] $Color = [ConsoleColor]::Gray
  )

  Write-Host $Message -ForegroundColor $Color
}

function Refresh-Path {
  $machinePath = [Environment]::GetEnvironmentVariable("Path", "Machine")
  $userPath = [Environment]::GetEnvironmentVariable("Path", "User")
  $env:Path = "$machinePath;$userPath"
}

function Test-Command {
  param([string] $Name)

  return $null -ne (Get-Command $Name -ErrorAction SilentlyContinue)
}

function Install-WinGetPackage {
  param(
    [string] $Command,
    [string] $PackageId
  )

  if (Test-Command $Command) {
    Write-Status "$Command is already installed." Green
    return
  }

  Write-Status "Installing $PackageId..." Cyan
  winget install --id $PackageId --exact --accept-source-agreements --accept-package-agreements
  if ($LASTEXITCODE) {
    throw "WinGet failed to install $PackageId."
  }

  Refresh-Path
}

if (-not (Test-Command "winget")) {
  throw "WinGet is required. Install App Installer from the Microsoft Store, then run this script again."
}

Install-WinGetPackage -Command "opencode" -PackageId "SST.opencode"
Install-WinGetPackage -Command "nvm" -PackageId "CoreyButler.NVMforWindows"

# Coreutils provides common Unix tools used by coding agents, such as sed.
Install-WinGetPackage -Command "sed" -PackageId "Microsoft.Coreutils"

Refresh-Path

Write-Status "Installing and activating the Node.js LTS release..." Cyan
nvm install lts
if ($LASTEXITCODE) {
  throw "NVM failed to install the Node.js LTS release."
}

nvm use lts
if ($LASTEXITCODE) {
  throw "NVM failed to activate the Node.js LTS release."
}

Start-Sleep -Seconds 1
Refresh-Path

$commands = @("opencode", "node", "npm", "npx")
foreach ($command in $commands) {
  if (-not (Test-Command $command)) {
    throw "Required command is not available after installation: $command"
  }
}

Write-Status "OpenCode installation verified:" Green
Write-Host "  opencode: $(opencode --version)"
Write-Host "  node:     $(node --version)"
Write-Host "  npm:      $(npm --version)"
Write-Host "  npx:      $(npx --version)"
