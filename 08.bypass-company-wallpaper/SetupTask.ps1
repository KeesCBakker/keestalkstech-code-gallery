& {
    $TaskName = "ChangeWallpaperOnLogin"
    $PicturesFolder = [Environment]::GetFolderPath('MyPictures')
    $ScriptPath = "$PicturesFolder\ChangeWallpaper.ps1"  # Path to the wallpaper-changing script
    $CurrentUser = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
    $ErrorActionPreference = "Stop"  # Ensure the script stops on errors

    # Ensure the script is running as administrator
    $isAdmin = [Security.Principal.WindowsIdentity]::GetCurrent().Groups -contains 'S-1-5-32-544'
    if (-not $isAdmin) {
        Write-Output "This script must be run as an administrator. Please restart it with administrative privileges."
        exit 1
    }

    # Check if the task already exists
    if (Get-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue) {
        Write-Output "Task '$TaskName' already exists. Deleting it now..."
        Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false
        Write-Output "Task '$TaskName' has been deleted."
    }

    # Define the task action
    $Action = New-ScheduledTaskAction `
        -Execute 'PowerShell.exe' `
        -Argument "-NoProfile -ExecutionPolicy Bypass -File `"$ScriptPath`""

    # Create a logon trigger for the current user
    $Trigger = New-ScheduledTaskTrigger -AtLogOn -User $CurrentUser

    # allow the task to run on batteries
    $Settings = New-ScheduledTaskSettingsSet `
        -AllowStartIfOnBatteries `
        -DontStopIfGoingOnBatteries

    # Register the task without a delay
    Register-ScheduledTask -TaskName $TaskName `
        -Action $Action `
        -Trigger $Trigger `
        -Description "Runs the ChangeWallpaper script at user logon." `
        -Settings $Settings

    # Add a 30-second delay to the task using XML configuration
    $Task = Get-ScheduledTask -TaskName $TaskName
    $Task.Settings.StartWhenAvailable = $true
    $Task.Triggers[0].Delay = 'PT30S'  # 30-second delay in ISO 8601 duration format
    $Task | Set-ScheduledTask

    Write-Output ""
    Write-Output "Scheduled task '$TaskName' has been created successfully with a 30-second delay and is allowed to run on battery power."
}
