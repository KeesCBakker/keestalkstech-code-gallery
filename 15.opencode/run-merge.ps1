param(
  [string] $Ref = "main"
)

$ErrorActionPreference = "Stop"
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

$repository = "KeesCBakker/keestalkstech-code-gallery"
$sourceDirectory = "15.opencode"
$temporaryDirectory = Join-Path $env:TEMP "opencode-merge-$([guid]::NewGuid())"
$files = @(
  "merge-config.ps1"
  "config/opencode.jsonc"
  "config/opencode-mcps.jsonc"
  "config/opencode-ask.jsonc"
  "config/opencode-config-files.jsonc"
  "config/opencode-watcher.jsonc"
)

try {
  New-Item -ItemType Directory -Force -Path $temporaryDirectory | Out-Null
  New-Item -ItemType Directory -Force -Path (Join-Path $temporaryDirectory "config") | Out-Null

  foreach ($file in $files) {
    $destination = Join-Path $temporaryDirectory $file
    $parent = Split-Path -Parent $destination
    New-Item -ItemType Directory -Force -Path $parent | Out-Null

    $url = "https://raw.githubusercontent.com/$repository/$Ref/$sourceDirectory/$file"
    Write-Host "Downloading $file"
    Invoke-WebRequest -Uri $url -OutFile $destination -UseBasicParsing
  }

  & (Join-Path $temporaryDirectory "merge-config.ps1")
  if ($LASTEXITCODE) {
    throw "The downloaded merge script failed with exit code $LASTEXITCODE."
  }
}
finally {
  Remove-Item -LiteralPath $temporaryDirectory -Recurse -Force -ErrorAction SilentlyContinue
}
