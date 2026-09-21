$ErrorActionPreference = "Stop"

$skills = @(
  @{
    Name = "skill-creator"
    Source = "https://github.com/anthropics/skills"
  }
  @{
    Name = "htmx"
    Source = "https://github.com/mindrally/skills"
  }
  @{
    Name = "find-skills"
    Source = "https://github.com/vercel-labs/skills"
  }
  @{
    Name = "git-commit"
    Source = "https://github.com/github/awesome-copilot"
  }
)

$npx = Get-Command npx -CommandType Application -ErrorAction SilentlyContinue
if (-not $npx) {
  throw "npx is required to install skills."
}

Write-Host "Checking installed global OpenCode skills..."
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
  $escapedName = [regex]::Escape($Name)
  return $installedText -match "(?im)(^|\s)$escapedName(\s|$)"
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
