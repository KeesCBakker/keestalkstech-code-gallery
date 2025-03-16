& {
  $day = "Monday"
  $time = "9:30am"
  $taskName = "UpgradeChocolateyPackages"

  # Check if the task exists:
  $taskExists = Get-ScheduledTask | Where-Object { $_.TaskName -eq $taskName }

  if ($taskExists) {
      # Unregister the existing task with the same name:
      Unregister-ScheduledTask -TaskName $taskName -Confirm:$false
  }

  # Register task the launches in an elevated powershell, user must confirm action
  $action = New-ScheduledTaskAction `
      -Execute "powershell.exe" `
      -Argument "-Command ""Start-Process powershell -ArgumentList '-Command & {choco upgrade all -y}' -Verb RunAs"""

  $trigger = New-ScheduledTaskTrigger `
      -Weekly `
      -DaysOfWeek $day `
      -At $time

  $settings = New-ScheduledTaskSettingsSet `
      -AllowStartIfOnBatteries `
      -DontStopIfGoingOnBatteries `
      -StartWhenAvailable `
      -RunOnlyIfNetworkAvailable

  # register task using all the setting
  Register-ScheduledTask `
      -Action $action `
      -Trigger $trigger `
      -TaskName $taskName `
      -Description "Upgrade all Chocolatey packages" `
      -Settings $settings
}
