dotnet tool install --global Microsoft.OpenApi.Kiota

# Go to our folder with clients
pushd "Ktt.KiotaAndResilience/HttpClients"

# Define the namespace by convention
$Namespace = "Ktt.KiotaAndResilience.HttpClients"

# Function to generate a Kiota client
function Generate-KiotaClient {
    param (
        [string]$Url,
        [string]$Name,
        [string]$Filter
    )

    $ClientNameSpace = "$Namespace.$Name"
    $Client          = "${Name}Client"

    kiota generate `
        --openapi "$Url" `
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
  -Url "https://petstore.swagger.io/v2/swagger.json" `
  -Name "PetStore" `
  -Filter "/pet/**"

popd
