& {
  # check if task exists
  $taskName = "UpgradeChocolateyPackages";
  $taskExists = Get-ScheduledTask | Where-Object { $_.TaskName -eq $taskName };
  if (-not $taskExists) {
      Write-Host "Task '$taskName' does not exist. Exiting." -ForegroundColor Red;
      exit 1;
  };

  Unregister-ScheduledTask -TaskName $taskName -Confirm:$false;

  $makeMeAdminDir = "C:\Program Files\Make Me Admin\";

  $newAction = New-ScheduledTaskAction `
      -Execute "powershell.exe" `
      -Argument "-Command ""cd '$makeMeAdminDir'; ./MakeMeAdminUI.exe; Read-Host 'Make sure you are admin before proceeding...'; Start-Process powershell -ArgumentList '-Command & {choco upgrade all -y}' -Verb RunAs"""; `

  Register-ScheduledTask `
      -Action $newAction `
      -Trigger $taskExists.Triggers `
      -TaskName $taskName `
      -Description "Upgrade all Chocolatey packages with admin prompt" `
      -Settings $taskExists.Settings;

  Write-Host "Task '$taskName' has been updated with the new action."
}
