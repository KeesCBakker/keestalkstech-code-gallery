param(
  [string] $Ref = "main"
)

$ErrorActionPreference = "Stop"
if ($Ref.StartsWith("-") -or $Ref.Contains("..") -or $Ref -notmatch '^[A-Za-z0-9._/-]+$') {
  throw "Invalid Git ref: $Ref"
}

$directory = Join-Path ([IO.Path]::GetTempPath()) "opencode-bootstrap-$([guid]::NewGuid())"
$files = @("package.json", "bun.lock", ".prettierrc", "src/merge-config.ts")

try {
  New-Item -ItemType Directory -Path $directory | Out-Null
  foreach ($file in $files) {
    $destination = Join-Path $directory $file
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $destination) | Out-Null
    Invoke-WebRequest -Uri "https://raw.githubusercontent.com/KeesCBakker/keestalkstech-code-gallery/$Ref/15.opencode/$file" -OutFile $destination
  }

  Push-Location $directory
  & bun install --frozen-lockfile --production
  if ($LASTEXITCODE -ne 0) {
    throw "Could not install the pinned merge dependencies (exit code $LASTEXITCODE)."
  }

  & bun run "src/merge-config.ts" --ref $Ref
  if ($LASTEXITCODE -ne 0) {
    throw "The merge program failed with exit code $LASTEXITCODE."
  }
}
finally {
  if ((Get-Location).Path -eq $directory) {
    Pop-Location
  }
  Remove-Item -LiteralPath $directory -Recurse -Force -ErrorAction SilentlyContinue
}
