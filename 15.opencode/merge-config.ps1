$ErrorActionPreference = "Stop"

$npx = Get-Command npx -CommandType Application -ErrorAction SilentlyContinue
if (-not $npx) {
  throw "npx is required. Install Node.js, then run this script again."
}

function Read-JsoncFile {
  param([string] $Path)

  if (-not (Test-Path -LiteralPath $Path)) {
    return @{}
  }

  return Get-Content -Raw -LiteralPath $Path | ConvertFrom-Json -AsHashtable
}

function Convert-ToHashtable {
  param($Value)

  if ($Value -is [hashtable]) {
    return $Value
  }

  $result = @{}
  foreach ($property in $Value.PSObject.Properties) {
    $result[$property.Name] = $property.Value
  }
  return $result
}

function Test-Equal {
  param($Left, $Right)

  return (($Left | ConvertTo-Json -Depth 100 -Compress) -eq
    ($Right | ConvertTo-Json -Depth 100 -Compress))
}

function Get-Count {
  param($Value)

  if ($null -eq $Value) {
    return 0
  }
  return @($Value).Count
}

function Get-ValueSummary {
  param($Value)

  if ($Value -is [hashtable]) {
    return "object ($($Value.Keys.Count) properties)"
  }
  if ($Value -is [array]) {
    return "array ($($Value.Count) items)"
  }
  if ($Value -is [bool]) {
    return [string]$Value
  }
  if ($null -eq $Value) {
    return "null"
  }
  return "value ($($Value.GetType().Name))"
}

function Get-SafeValue {
  param(
    [string] $Name,
    $Value
  )

  if ($Name -match '(?i)(secret|token|password|api.?key|private.?key|credential)') {
    return "<sensitive>"
  }
  if ($Value -is [hashtable]) {
    $parts = foreach ($property in $Value.GetEnumerator() | Sort-Object Key) {
      "{0}={1}" -f $property.Key, (Get-SafeValue -Name $property.Key -Value $property.Value)
    }
    return "{ " + ($parts -join ", ") + " }"
  }
  if ($Value -is [array]) {
    return "[" + (($Value | ForEach-Object { Get-SafeValue -Name $Name -Value $_ }) -join ", ") + "]"
  }
  if ($null -eq $Value) {
    return "null"
  }
  return [string]$Value
}

function Get-ConflictSummary {
  param(
    [string] $Path,
    $Value
  )

  if ($Value -is [hashtable]) {
    return Get-SafeValue -Name $Path -Value $Value
  }
  return Get-SafeValue -Name $Path -Value $Value
}

function Show-Preflight {
  param(
    [hashtable] $Central,
    [hashtable] $Fragments,
    [string] $ConfigPath
  )

  Write-Host "`nOpenCode merge preflight" -ForegroundColor Cyan
  Write-Host "Central config: $ConfigPath"

  foreach ($fragment in $Fragments.GetEnumerator()) {
    $name = $fragment.Key
    $path = $fragment.Value.Path
    $status = if ($fragment.Value.Exists) { "found" } else { "missing" }
    Write-Host ("  {0,-30} {1}" -f $name, $status)
  }

  $centralBash = if ($Central.permission -and $Central.permission.bash) { $Central.permission.bash } else { @{} }
  $centralRead = if ($Central.permission -and $Central.permission.read) { $Central.permission.read } else { @{} }
  $centralWatcher = if ($Central.watcher -and $Central.watcher.ignore) { $Central.watcher.ignore } else { @() }
  $centralMcp = if ($Central.mcp) { $Central.mcp } else { @{} }

  $ask = $Fragments["opencode-ask.jsonc"].Data
  $configFiles = $Fragments["opencode-config-files.jsonc"].Data
  $watcher = $Fragments["opencode-watcher.jsonc"].Data
  $mcps = $Fragments["opencode-mcps.jsonc"].Data

  $projectBash = if ($ask.permission.bash) { $ask.permission.bash } else { @{} }
  $projectRead = if ($configFiles.permission.read) { $configFiles.permission.read } else { @{} }
  $projectWatcher = if ($watcher.watcher.ignore) { $watcher.watcher.ignore } else { @() }
  $projectMcp = if ($mcps.mcp) { $mcps.mcp } else { @{} }

  Write-Host "`nSections"
  Write-Host ("  Bash rules:       central {0,3} | project {1,3}" -f (Get-Count $centralBash.Keys), (Get-Count $projectBash.Keys))
  Write-Host ("  Read rules:       central {0,3} | project {1,3}" -f (Get-Count $centralRead.Keys), (Get-Count $projectRead.Keys))
  Write-Host ("  Watcher patterns: central {0,3} | project {1,3}" -f (Get-Count $centralWatcher), (Get-Count $projectWatcher))
  Write-Host ("  MCPs:             central {0,3} | project {1,3}" -f (Get-Count $centralMcp.Keys), (Get-Count $projectMcp.Keys))

  Write-Host "`nMCP status"
  foreach ($mcp in $projectMcp.GetEnumerator()) {
    $state = if ($centralMcp.ContainsKey($mcp.Key)) { "configured" } else { "not configured" }
    $enabled = if ($mcp.Value.enabled -eq $true) { "enabled" } else { "disabled" }
    Write-Host ("  {0,-20} {1,-16} project {2}" -f $mcp.Key, $state, $enabled)
  }
  Write-Host ""
}

function Convert-ToEdiktPath {
  param([string] $Prefix, [string] $Name)

  $key = $Name | ConvertTo-Json -Compress
  return "$Prefix[$key]"
}

function Convert-ToEdiktValue {
  param($Value)

  # Keep inserted objects and arrays readable in the Edikt script.
  return $Value | ConvertTo-Json -Depth 100
}

function Add-EdiktObjectExpressions {
  param(
    [string] $Path,
    [hashtable] $Value,
    [System.Collections.Generic.List[string]] $Expressions
  )

  foreach ($property in $Value.GetEnumerator()) {
    $key = $property.Key | ConvertTo-Json -Compress
    $propertyPath = "$Path[$key]"
    if ($property.Value -is [hashtable]) {
      Add-EdiktObjectExpressions -Path $propertyPath -Value $property.Value -Expressions $Expressions
    }
    else {
      $Expressions.Add("$propertyPath = $(Convert-ToEdiktValue $property.Value)")
    }
  }
}

function Confirm-Conflict {
  param(
    [string] $Path,
    $Current,
    $Incoming
  )

  Write-Host "`nConflict: $Path" -ForegroundColor Yellow
  Write-Host "Current:  $(Get-ConflictSummary -Path $Path -Value $Current)"
  Write-Host "Incoming: $(Get-ConflictSummary -Path $Path -Value $Incoming)"

  do {
    $answer = Read-Host "Use incoming value? [y/N]"
  } while ($answer -notmatch '^(|y|yes|n|no)$')

  return $answer -match '^(y|yes)$'
}

function Merge-Object {
  param(
    [hashtable] $Target,
    [hashtable] $Incoming,
    [string] $Prefix,
    [System.Collections.Generic.List[string]] $Expressions
  )

  foreach ($property in $Incoming.GetEnumerator()) {
    $key = $property.Key | ConvertTo-Json -Compress
    $path = if ($Prefix) { "$Prefix[$key]" } else { ".$key" }
    $currentExists = $Target.ContainsKey($property.Key)
    $incomingValue = $property.Value

    if (-not $currentExists) {
      $Target[$property.Key] = $incomingValue
      $Expressions.Add("$path = $(Convert-ToEdiktValue $incomingValue)")
      continue
    }

    $currentValue = $Target[$property.Key]
    if ($currentValue -is [hashtable] -and $incomingValue -is [hashtable]) {
      Merge-Object -Target $currentValue -Incoming $incomingValue -Prefix $path -Expressions $Expressions
      continue
    }

    if (Test-Equal $currentValue $incomingValue) {
      continue
    }

    if (Confirm-Conflict -Path $path -Current $currentValue -Incoming $incomingValue) {
      $Target[$property.Key] = $incomingValue
      $Expressions.Add("$path = $(Convert-ToEdiktValue $incomingValue)")
    }
  }
}

function Merge-Array {
  param(
    [hashtable] $Target,
    [hashtable] $Incoming,
    [string] $Property,
    [string] $Path,
    [System.Collections.Generic.List[string]] $Expressions
  )

  if (-not $Target.ContainsKey($Property)) {
    $Target[$Property] = @($Incoming[$Property])
    $Expressions.Add(".$(($Path -split '\.')[0])[$($Property | ConvertTo-Json -Compress)] = $(Convert-ToEdiktValue @($Incoming[$Property]))")
    return
  }

  $current = @($Target[$Property])
  $original = @($current)
  foreach ($item in @($Incoming[$Property])) {
    if ($current -notcontains $item) {
      $current += $item
    }
  }
  $Target[$Property] = $current
  if (-not (Test-Equal $original $current)) {
    $Expressions.Add(".$(($Path -split '\.')[0])[$($Property | ConvertTo-Json -Compress)] = $(Convert-ToEdiktValue $current)")
  }
}

function Save-SecretReferences {
  param(
    [hashtable] $Config,
    [string] $ConfigDirectory
  )

  if (-not $Config.ContainsKey("mcp")) {
    return
  }

  $secretDirectory = Join-Path $ConfigDirectory "secrets"
  foreach ($mcp in $Config.mcp.GetEnumerator()) {
    $json = $mcp.Value | ConvertTo-Json -Depth 100 -Compress
    $references = [regex]::Matches($json, '\{file:([^}]+)\}')

    foreach ($reference in $references) {
      $relativePath = $reference.Groups[1].Value
      if ($relativePath.StartsWith("./")) {
        $relativePath = $relativePath.Substring(2)
      }
      $secretPath = Join-Path $ConfigDirectory $relativePath

      if (Test-Path -LiteralPath $secretPath) {
        Write-Host "Secret file already exists; leaving it unchanged: $secretPath"
        continue
      }

      $secretName = Split-Path $secretPath -Leaf
      $value = Read-Host "Enter value for MCP secret '$secretName' (leave empty to keep the MCP unusable)" -AsSecureString
      $credential = [System.Net.NetworkCredential]::new("secret", $value)
      $plainText = $credential.Password

      if ([string]::IsNullOrWhiteSpace($plainText)) {
        continue
      }

      $parent = Split-Path $secretPath -Parent
      New-Item -ItemType Directory -Force -Path $parent | Out-Null

      try {
        # CreateNew fails instead of overwriting if another process created it.
        $stream = [System.IO.File]::Open(
          $secretPath,
          [System.IO.FileMode]::CreateNew,
          [System.IO.FileAccess]::Write,
          [System.IO.FileShare]::None
        )
        try {
          $bytes = [System.Text.UTF8Encoding]::new($false).GetBytes($plainText)
          $stream.Write($bytes, 0, $bytes.Length)
        }
        finally {
          $stream.Dispose()
        }
        Write-Host "Created secret file $secretPath"
      }
      catch [System.IO.IOException] {
        Write-Host "Secret file was created concurrently; leaving it unchanged: $secretPath"
      }
    }
  }
}

$projectDirectory = Join-Path $PSScriptRoot "config"
$configDirectory = Join-Path $HOME ".config\opencode"
$jsoncPath = Join-Path $configDirectory "opencode.jsonc"
$jsonPath = Join-Path $configDirectory "opencode.json"
$centralPath = @($jsoncPath, $jsonPath) | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $centralPath) {
  throw "No central OpenCode config found in $configDirectory."
}

$backupPath = "$centralPath.backup.$(Get-Date -Format 'yyyyMMdd_HHmmss_fff')"
Copy-Item -LiteralPath $centralPath -Destination $backupPath
Write-Host "Backup created at $backupPath"

$merged = Read-JsoncFile -Path $centralPath
$expressions = [System.Collections.Generic.List[string]]::new()
$fragmentNames = @(
  "opencode.jsonc"
  "opencode-mcps.jsonc"
  "opencode-ask.jsonc"
  "opencode-config-files.jsonc"
  "opencode-watcher.jsonc"
)

$fragmentInventory = @{}
foreach ($fragmentName in $fragmentNames) {
  $fragmentPath = Join-Path $projectDirectory $fragmentName
  $exists = Test-Path -LiteralPath $fragmentPath
  $fragmentInventory[$fragmentName] = @{
    Path = $fragmentPath
    Exists = $exists
    Data = if ($exists) { Read-JsoncFile -Path $fragmentPath } else { @{} }
  }
}

Show-Preflight -Central $merged -Fragments $fragmentInventory -ConfigPath $centralPath

foreach ($fragmentName in $fragmentNames) {
  $fragment = $fragmentInventory[$fragmentName].Data

  if ($fragmentName -eq "opencode-mcps.jsonc" -and $fragment.ContainsKey("mcp")) {
    if (-not $merged.ContainsKey("mcp")) {
      $merged.mcp = @{}
    }

    foreach ($mcp in $fragment.mcp.GetEnumerator()) {
      if (-not $merged.mcp.ContainsKey($mcp.Key)) {
        $answer = Read-Host "MCP '$($mcp.Key)' is not configured. Add it? [y/N]"
        if ($answer -match '^(y|yes)$') {
          $merged.mcp[$mcp.Key] = $mcp.Value
          $mcpPath = ".mcp[$($mcp.Key | ConvertTo-Json -Compress)]"
          Add-EdiktObjectExpressions -Path $mcpPath -Value $mcp.Value -Expressions $expressions
        }
      }
      elseif (-not (Test-Equal $merged.mcp[$mcp.Key] $mcp.Value)) {
        $answer = Read-Host "MCP '$($mcp.Key)' is already configured. Overwrite it with the project version? [y/N]"
        if ($answer -match '^(y|yes)$') {
          $merged.mcp[$mcp.Key] = $mcp.Value
          $mcpPath = ".mcp[$($mcp.Key | ConvertTo-Json -Compress)]"
          $expressions.Add("del($mcpPath)")
          Add-EdiktObjectExpressions -Path $mcpPath -Value $mcp.Value -Expressions $expressions
        }
      }
    }

    continue
  }

  if ($fragmentName -eq "opencode-watcher.jsonc" -and $fragment.ContainsKey("watcher")) {
    if (-not $merged.ContainsKey("watcher")) {
      $merged.watcher = @{}
    }
    Merge-Array -Target $merged.watcher -Incoming $fragment.watcher -Property "ignore" -Path "watcher.ignore" -Expressions $expressions
    continue
  }

  $fragment.Remove("`$schema")
  Merge-Object -Target $merged -Incoming $fragment -Prefix "" -Expressions $expressions
}

Save-SecretReferences -Config $merged -ConfigDirectory $configDirectory

if ($expressions.Count -eq 0) {
  Write-Host "No configuration changes detected; leaving the central config unchanged."
  exit 0
}

$ediktVersion = "v0.5.0"
$ediktDirectory = Join-Path $env:TEMP "opencode-edikt-$([guid]::NewGuid())"
$ediktPath = Join-Path $ediktDirectory "edikt.exe"

$ediktZip = Join-Path $ediktDirectory "edikt-$ediktVersion.zip"
New-Item -ItemType Directory -Force -Path $ediktDirectory | Out-Null
Invoke-WebRequest "https://github.com/jhheider/edikt/releases/download/$ediktVersion/edikt-windows-x86_64.zip" -OutFile $ediktZip
Expand-Archive -Force -Path $ediktZip -DestinationPath $ediktDirectory
Remove-Item -LiteralPath $ediktZip -Force

$temporaryPath = "$centralPath.tmp.$([guid]::NewGuid()).jsonc"
Copy-Item -LiteralPath $centralPath -Destination $temporaryPath
$ediktScript = Join-Path $env:TEMP "opencode-merge-$([guid]::NewGuid()).edk"
$ediktText = $expressions -join " |`n"
[System.IO.File]::WriteAllText($ediktScript, $ediktText, [System.Text.UTF8Encoding]::new($false))

$previousConfig = $env:OPENCODE_CONFIG
$env:OPENCODE_CONFIG = $temporaryPath
try {
  if ($expressions.Count -gt 0) {
    $output = & $ediktPath -t jsonc -f $ediktScript --in-place -- $temporaryPath 2>&1
    if ($LASTEXITCODE) {
      throw "edikt failed: $(($output -join ' ') -replace '\s+', ' ')"
    }
  }

  $prettier = Get-Command prettier -CommandType Application -ErrorAction SilentlyContinue
  $prettierCommand = $null
  $prettierArguments = @()

  if ($prettier) {
    $prettierCommand = $prettier.Path
    $prettierArguments = @("--write", "--parser", "jsonc", $temporaryPath)
    Write-Host "Formatting merged config with prettier."
  }
  else {
    $npx = Get-Command npx -CommandType Application -ErrorAction SilentlyContinue
    if ($npx) {
      # nvm-windows can expose npx through a shim whose Path contains
      # both the wrapper and target. Let PowerShell resolve the command name.
      $prettierCommand = "npx.cmd"
      $prettierArguments = @("--yes", "prettier", "--write", "--parser", "jsonc", $temporaryPath)
      Write-Host "Formatting merged config with prettier via npx. Prettier may be downloaded if needed."
    }
  }

  if (-not $prettierCommand) {
    Write-Verbose "Neither prettier nor npx was found; continuing without formatting."
  }
  else {
    $prettierOutput = & $prettierCommand @prettierArguments 2>&1
    if ($LASTEXITCODE) {
      Write-Verbose "Prettier failed; continuing without formatting."
    }
  }

  & opencode debug config *> $null
  if ($LASTEXITCODE) {
    throw "OpenCode rejected the merged configuration."
  }

  Move-Item -LiteralPath $temporaryPath -Destination $centralPath -Force
  Write-Host "Merged configuration written to $centralPath"

  $skillInstaller = Join-Path $PSScriptRoot "install-skills.ps1"
  if (Test-Path -LiteralPath $skillInstaller) {
    Write-Host "Checking optional OpenCode skills."
    try {
      & $skillInstaller
      if ($LASTEXITCODE) {
        Write-Warning "The config merge succeeded, but the skill installer failed with exit code $LASTEXITCODE."
      }
    }
    catch {
      Write-Warning "The config merge succeeded, but the skill installer failed: $($_.Exception.Message)"
    }
  }
}
catch {
  if (Test-Path -LiteralPath $temporaryPath) {
    Remove-Item -LiteralPath $temporaryPath -Force -ErrorAction SilentlyContinue
    Write-Warning "The central config was not changed. Backup remains at $backupPath"
  }
  throw
}
finally {
  Remove-Item -LiteralPath $ediktScript -Force -ErrorAction SilentlyContinue
  Remove-Item -LiteralPath $ediktDirectory -Recurse -Force -ErrorAction SilentlyContinue
  $env:OPENCODE_CONFIG = $previousConfig
}
