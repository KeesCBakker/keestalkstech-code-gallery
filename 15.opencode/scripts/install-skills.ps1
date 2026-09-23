$ErrorActionPreference = "Stop"

function Write-Status {
  param(
    [string] $Message,
    [ConsoleColor] $Color = [ConsoleColor]::Gray
  )

  Write-Host $Message -ForegroundColor $Color
}

$skillsPath = Join-Path (Split-Path -Parent $PSScriptRoot) "config\opencode-skills.yaml"
if (-not (Test-Path -LiteralPath $skillsPath)) {
  throw "Skills configuration not found: $skillsPath"
}

# Parse the intentionally small, dependency-free YAML structure used here.
$skills = @()
$currentSkill = $null
foreach ($line in Get-Content -LiteralPath $skillsPath) {
  if ($line -match '^\s*-\s+name:\s*(.+?)\s*$') {
    $currentSkill = @{ Name = $Matches[1].Trim(); Source = $null }
    $skills += $currentSkill
    continue
  }
  if ($line -match '^\s+source:\s*(.+?)\s*$' -and $null -ne $currentSkill) {
    $currentSkill.Source = $Matches[1].Trim()
  }
}

if ($skills.Count -eq 0 -or @($skills | Where-Object { [string]::IsNullOrWhiteSpace($_.Name) -or [string]::IsNullOrWhiteSpace($_.Source) }).Count -gt 0) {
  throw "Skills configuration is empty or invalid: $skillsPath"
}

$bun = Get-Command bun -CommandType Application -ErrorAction SilentlyContinue
if (-not $bun) {
  throw "bun is required to install skills."
}

Write-Status "Checking installed global OpenCode skills..." Cyan
Write-Status "`nConfigured skills:" Cyan
foreach ($skill in $skills) {
  Write-Status ("  {0,-20} {1}" -f $skill.Name, $skill.Source) DarkGray
}

function Get-InstalledSkills {
  $output = & bun x skills list --global --agent opencode 2>&1
  if ($LASTEXITCODE) {
    throw "Could not list installed skills: $($output -join ' ')"
  }
  return $output -join "`n"
}

function Test-SkillInstalled {
  param([string] $Name)

  $installedText = Get-InstalledSkills
  # The skills CLI uses ANSI color codes around skill names.
  $plainText = $installedText -replace "`e\[[0-9;]*m", ""
  $escapedName = [regex]::Escape($Name)
  return $plainText -match "(?im)^\s*$escapedName\s+"
}

foreach ($skill in $skills) {
  if (Test-SkillInstalled -Name $skill.Name) {
    Write-Status "Already installed: $($skill.Name)" Green
    continue
  }

  $answer = Read-Host "Install '$($skill.Name)' globally for OpenCode? [y/N]"
  if ($answer -notmatch '^(y|yes)$') {
    Write-Status "Skipped: $($skill.Name)" Yellow
    continue
  }

  Write-Status "Installing: $($skill.Name)" Cyan
  & bun x skills add $skill.Source --skill $skill.Name --global --agent opencode --yes
  if ($LASTEXITCODE) {
    throw "Failed to install skill '$($skill.Name)'."
  }

  if (-not (Test-SkillInstalled -Name $skill.Name)) {
    throw "Skill '$($skill.Name)' was reported as installed but is not listed for OpenCode."
  }
  Write-Status "Verified installed: $($skill.Name)" Green
}

Write-Status "`nInstalled global OpenCode skills:" Cyan
& bun x skills list --global --agent opencode
