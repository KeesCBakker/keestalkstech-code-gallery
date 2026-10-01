& {
  $exportPath = Join-Path $env:TEMP "winget-packages-$([guid]::NewGuid()).json"

  try {
    $exportOutput = & winget export --output $exportPath --disable-interactivity 2>&1
    $exitCode = $LASTEXITCODE

    if ($exitCode -ne 0) {
      throw "WinGet export failed (exit code ${exitCode}):`n$($exportOutput -join [Environment]::NewLine)"
    }

    $export = Get-Content -LiteralPath $exportPath -Raw |
      ConvertFrom-Json -ErrorAction Stop

    foreach ($source in ($export.Sources | Sort-Object { $_.SourceDetails.Name })) {
      $sourceName = $source.SourceDetails.Name.Replace("'", "''")

      foreach ($package in ($source.Packages | Sort-Object PackageIdentifier)) {
        $packageId = $package.PackageIdentifier.Replace("'", "''")
        'winget install --id ''{0}'' --exact --source ''{1}''' -f $packageId, $sourceName
      }
    }
  }
  finally {
    Remove-Item -LiteralPath $exportPath -Force -ErrorAction SilentlyContinue
  }
}
