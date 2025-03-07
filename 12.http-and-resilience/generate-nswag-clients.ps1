# Ensure NSwag is installed
if (-not (Get-Command "nswag" -ErrorAction SilentlyContinue)) {
    Write-Host "NSwag is not installed. Installing NSwag..."
    dotnet tool install --global NSwag.ConsoleCore
}

nswag run petstore.nswag.json
nswag run httpstatus.nswag.json
