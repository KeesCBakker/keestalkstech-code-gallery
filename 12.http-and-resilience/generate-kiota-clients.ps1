# Ensure Kiota is installed
if (-not (Get-Command "kiota" -ErrorAction SilentlyContinue)) {
    Write-Host "Kiota is not installed. Installing Kiota..."
    dotnet tool install --global Microsoft.OpenApi.Kiota
}

# Go to our folder with the project
pushd "Ktt.Resilience.Clients.Kiota"

# Install bundle
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions
dotnet add package Microsoft.Extensions.Options
dotnet add package Microsoft.Extensions.Http.Resilience
dotnet add package Microsoft.Extensions.Resilience
dotnet add package Microsoft.Kiota.Bundle

# let's goto the
pushd "HttpClients"

# Define the namespace by convention
$Namespace = "Ktt.Resilience.Clients.Kiota.HttpClients"

# Function to generate a Kiota client
function Generate-KiotaClient {
    param (
        [string]$Location,
        [string]$Name,
        [string]$Filter="/**"
    )

    $ClientNameSpace = "$Namespace.$Name"
    $Client          = "${Name}Client"

    kiota generate `
        --openapi "$Location" `
        --language CSharp `
        --output "$Name" `
        --namespace-name "$ClientNameSpace" `
        --class-name "$Client" `
        --exclude-backward-compatible `
        --clean-output `
        --clear-cache `
        --include-path "$Filter"

    echo ""
}

# Generate clients

Generate-KiotaClient `
  -Location "https://petstore.swagger.io/v2/swagger.json" `
  -Name "PetStore" `
  -Filter "/pet/**"

Generate-KiotaClient `
  -Location ../../httpstatus-open-api.yml `
  -Name "HttpStatus"

popd
