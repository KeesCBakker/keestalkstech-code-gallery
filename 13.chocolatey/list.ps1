choco list -l -r --id-only `
| Where-Object { $_ -notmatch '^(KB|chocolatey|vcredist)' } `
| ForEach-Object { echo "choco install -y $PSItem" }
