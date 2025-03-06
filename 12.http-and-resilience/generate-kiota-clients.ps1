dotnet tool install --global Microsoft.OpenApi.Kiota

# Go to our folder with clients
pushd "Ktt.Resilience.KiotaClients/HttpClients"

# Define the namespace by convention
$Namespace = "Ktt.Resilience.KiotaClients.HttpClients"

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
