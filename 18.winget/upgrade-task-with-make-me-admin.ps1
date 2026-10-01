& {
  $taskName = 'UpgradeWingetPackages'
  $taskExists = Get-ScheduledTask -TaskName $taskName -ErrorAction SilentlyContinue
  if (-not $taskExists) {
    throw "Task '$taskName' does not exist. Run upgrade-task.ps1 first."
  }

  $makeMeAdminDir = Join-Path $env:ProgramFiles 'Make Me Admin'
  $makeMeAdminPath = Join-Path $makeMeAdminDir 'MakeMeAdminUI.exe'
  $scriptPath = Join-Path $env:LOCALAPPDATA 'Upgrade-WingetPackages.ps1'

  if (-not (Test-Path -LiteralPath $makeMeAdminPath)) {
    throw "Make Me Admin was not found at '$makeMeAdminPath'."
  }

  if (-not (Test-Path -LiteralPath $scriptPath)) {
    throw "The WinGet upgrade script was not found at '$scriptPath'. Run upgrade-task.ps1 first."
  }

  $quotedMakeMeAdminDir = $makeMeAdminDir.Replace("'", "''")
  $quotedScriptPath = $scriptPath.Replace("'", "''")
  $launcher = @"
Set-Location -LiteralPath '$quotedMakeMeAdminDir'
& (Join-Path '$quotedMakeMeAdminDir' 'MakeMeAdminUI.exe')
Read-Host 'Make sure you are admin before proceeding...'
`$arguments = '-NoProfile -ExecutionPolicy Bypass -File `"$quotedScriptPath`"'
`$process = Start-Process -FilePath 'powershell.exe' -ArgumentList `$arguments -Verb RunAs -Wait -PassThru
exit `$process.ExitCode
"@
  $encodedLauncher = [Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes($launcher))

  $newAction = New-ScheduledTaskAction `
    -Execute 'powershell.exe' `
    -Argument "-NoProfile -EncodedCommand $encodedLauncher"

  Unregister-ScheduledTask -TaskName $taskName -Confirm:$false

  Register-ScheduledTask `
    -Action $newAction `
    -Trigger $taskExists.Triggers `
    -TaskName $taskName `
    -Description 'Upgrade all WinGet packages with Make Me Admin' `
    -Settings $taskExists.Settings `
    -Principal $taskExists.Principal
}
