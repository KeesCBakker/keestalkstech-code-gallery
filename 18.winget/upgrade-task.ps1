& {
  $day = 'Monday'
  $time = '9:30AM'
  $taskName = 'UpgradeWingetPackages'
  $scriptPath = Join-Path $env:LOCALAPPDATA 'Upgrade-WingetPackages.ps1'

  $upgradeScript = @'
winget source update
if ($LASTEXITCODE -ne 0) {
    throw "WinGet source update failed (exit code $LASTEXITCODE)."
}

winget upgrade --all `
    --silent `
    --disable-interactivity `
    --accept-source-agreements `
    --accept-package-agreements
if ($LASTEXITCODE -ne 0) {
    throw "WinGet upgrade failed (exit code $LASTEXITCODE)."
}
'@

  Set-Content -LiteralPath $scriptPath -Value $upgradeScript -Encoding UTF8

  $quotedScriptPath = $scriptPath.Replace("'", "''")
  $launcher = @"
`$arguments = '-NoProfile -ExecutionPolicy Bypass -File `"$quotedScriptPath`"'
`$process = Start-Process -FilePath 'powershell.exe' -ArgumentList `$arguments -Verb RunAs -Wait -PassThru
exit `$process.ExitCode
"@
  $encodedLauncher = [Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes($launcher))

  $action = New-ScheduledTaskAction `
    -Execute 'powershell.exe' `
    -Argument "-NoProfile -EncodedCommand $encodedLauncher"

  $trigger = New-ScheduledTaskTrigger `
    -Weekly `
    -DaysOfWeek $day `
    -At $time

  $settings = New-ScheduledTaskSettingsSet `
    -AllowStartIfOnBatteries `
    -DontStopIfGoingOnBatteries `
    -StartWhenAvailable `
    -RunOnlyIfNetworkAvailable

  $principal = New-ScheduledTaskPrincipal `
    -UserId ([Security.Principal.WindowsIdentity]::GetCurrent().Name) `
    -LogonType Interactive `
    -RunLevel Limited

  $taskExists = Get-ScheduledTask -TaskName $taskName -ErrorAction SilentlyContinue
  if ($taskExists) {
    Unregister-ScheduledTask -TaskName $taskName -Confirm:$false
  }

  Register-ScheduledTask `
    -Action $action `
    -Trigger $trigger `
    -TaskName $taskName `
    -Description 'Upgrade all WinGet packages' `
    -Settings $settings `
    -Principal $principal
}
