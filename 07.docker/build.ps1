$ErrorActionPreference = "Stop"

$imageName = "ktt-docker-todo"
$integrationImage = "$imageName-integration-test"

Write-Host "`n🔨 Building integration test image: $integrationImage`n"
docker build --target integration-test -t $integrationImage .
if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Failed to build integration test image."
    exit $LASTEXITCODE
}

Write-Host "`n🚀 Running integration tests in: $integrationImage`n"
docker run --rm --privileged $integrationImage
if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Integration tests failed."
    exit $LASTEXITCODE
}

Write-Host "`n📦 Building final image: $imageName`n"
docker build -t $imageName .
if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Failed to build final image."
    exit $LASTEXITCODE
}

Write-Host "`n✅ Done`n"
