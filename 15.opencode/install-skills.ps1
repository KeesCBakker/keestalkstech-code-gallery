$ErrorActionPreference = "Stop"

$skillsPath = Join-Path $PSScriptRoot "config\opencode-skills.yaml"
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

$npx = Get-Command npx -CommandType Application -ErrorAction SilentlyContinue
if (-not $npx) {
  throw "npx is required to install skills."
}

Write-Host "Checking installed global OpenCode skills..."
Write-Host "`nConfigured skills:"
foreach ($skill in $skills) {
  Write-Host ("  {0,-20} {1}" -f $skill.Name, $skill.Source)
}

function Get-InstalledSkills {
  $output = & npx.cmd skills list --global --agent opencode 2>&1
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
    Write-Host "Already installed: $($skill.Name)"
    continue
  }

  $answer = Read-Host "Install '$($skill.Name)' globally for OpenCode? [y/N]"
  if ($answer -notmatch '^(y|yes)$') {
    Write-Host "Skipped: $($skill.Name)"
    continue
  }

  Write-Host "Installing: $($skill.Name)"
  & npx.cmd skills add $skill.Source --skill $skill.Name --global --agent opencode --yes
  if ($LASTEXITCODE) {
    throw "Failed to install skill '$($skill.Name)'."
  }

  if (-not (Test-SkillInstalled -Name $skill.Name)) {
    throw "Skill '$($skill.Name)' was reported as installed but is not listed for OpenCode."
  }
  Write-Host "Verified installed: $($skill.Name)"
}

Write-Host "`nInstalled global OpenCode skills:"
& npx.cmd skills list --global --agent opencode
