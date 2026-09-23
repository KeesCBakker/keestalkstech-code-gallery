param(
  [string] $Ref = "main"
)

$ErrorActionPreference = "Stop"
if ($Ref.StartsWith("-") -or $Ref.Contains("..") -or $Ref -notmatch '^[A-Za-z0-9._/-]+$') {
  throw "Invalid Git ref: $Ref"
}

$directory = Join-Path ([IO.Path]::GetTempPath()) "opencode-bootstrap-$([guid]::NewGuid())"
$files = @("package.json", "bun.lock", ".prettierrc", "merge-config.ts")

try {
  New-Item -ItemType Directory -Path $directory | Out-Null
  foreach ($file in $files) {
    Invoke-WebRequest -Uri "https://raw.githubusercontent.com/KeesCBakker/keestalkstech-code-gallery/$Ref/15.opencode/$file" -OutFile (Join-Path $directory $file)
  }

  & bun install --frozen-lockfile --production --cwd $directory
  if ($LASTEXITCODE -ne 0) {
    throw "Could not install the pinned merge dependencies (exit code $LASTEXITCODE)."
  }

  & bun run (Join-Path $directory "merge-config.ts") --ref $Ref
  if ($LASTEXITCODE -ne 0) {
    throw "The merge program failed with exit code $LASTEXITCODE."
  }
}
finally {
  Remove-Item -LiteralPath $directory -Recurse -Force -ErrorAction SilentlyContinue
}
