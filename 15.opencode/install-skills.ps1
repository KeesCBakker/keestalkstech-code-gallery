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
$installedOutput = & npx.cmd skills list --global --agent opencode 2>&1
if ($LASTEXITCODE) {
  throw "Could not list installed skills: $($installedOutput -join ' ')"
}

$installedText = $installedOutput -join "`n"

foreach ($skill in $skills) {
  $escapedName = [regex]::Escape($skill.Name)
  if ($installedText -match "(?im)(^|\s)$escapedName(\s|$)") {
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
}

Write-Host "`nInstalled global OpenCode skills:"
& npx.cmd skills list --global --agent opencode
