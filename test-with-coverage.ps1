param(
    [switch]$IncludeIntegrationTests
)

$ErrorActionPreference = 'Stop'
$coverageDirectory = Join-Path $PSScriptRoot 'TestResults/coverage'

$testProjects = @(
    @{ Name = 'console-app-dependency-injection'; Path = '01.dependency-injection-with-ioptions-in-console-apps/Ktt.ConsoleAppDependencyInjection.Tests/Ktt.ConsoleAppDependencyInjection.Tests.csproj'; MinimumTests = 12 },
    @{ Name = 'jwt-secured-api'; Path = '02.simple-jwt-access-policies-for-api-security-in-net/Ktt.JwtSecuredApi.Tests/Ktt.JwtSecuredApi.Tests.csproj'; MinimumTests = 9 },
    @{ Name = 'roman-numerals'; Path = '05.roman-numerals/Ktt.RomanNumerals.Test/Ktt.RomanNumerals.Test.csproj'; MinimumTests = 14 },
    @{ Name = 'json-handlebars'; Path = '06.handlebars/Ktt.JsonHandlebars.Test/Ktt.JsonHandlebars.Test.csproj'; MinimumTests = 3 },
    @{ Name = 'docker-todo-api'; Path = '07.docker/test/Ktt.Docker.Todo.Api.Tests/Ktt.Docker.Todo.Api.Tests.csproj'; MinimumTests = 5; Filter = '/**[Category!=Integration]' },
    @{ Name = 'resilience'; Path = '12.http-and-resilience/Ktt.Resilience.Tests/Ktt.Resilience.Tests.csproj'; MinimumTests = 4 },
    @{ Name = 'validation-api'; Path = '14.validation/Ktt.Validation.Api.Tests/Ktt.Validation.Api.Tests.csproj'; MinimumTests = 85 }
)

New-Item -ItemType Directory -Path $coverageDirectory -Force | Out-Null
Remove-Item -Path (Join-Path $coverageDirectory '*.xml') -Force -ErrorAction SilentlyContinue

foreach ($testProject in $testProjects) {
    $projectPath = Join-Path $PSScriptRoot $testProject.Path
    $coveragePath = Join-Path $coverageDirectory "$($testProject.Name).cobertura.xml"
    $arguments = @(
        'run',
        '--project', $projectPath,
        '--configuration', 'Release',
        '--',
        '--coverage',
        '--coverage-output', $coveragePath,
        '--coverage-output-format', 'cobertura',
        '--minimum-expected-tests', $testProject.MinimumTests,
        '--disable-logo'
    )

    if ($testProject.Filter) {
        $arguments += @('--treenode-filter', $testProject.Filter)
    }

    & dotnet @arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Tests failed for $($testProject.Path)."
    }
}

if ($IncludeIntegrationTests) {
    $projectPath = Join-Path $PSScriptRoot '07.docker/test/Ktt.Docker.Todo.Api.Tests/Ktt.Docker.Todo.Api.Tests.csproj'
    $coveragePath = Join-Path $coverageDirectory 'docker-todo-api-integration.cobertura.xml'

    & dotnet run --project $projectPath --configuration Release -- `
        --coverage `
        --coverage-output $coveragePath `
        --coverage-output-format cobertura `
        --minimum-expected-tests 5 `
        --treenode-filter '/**[Category=Integration]' `
        --disable-logo

    if ($LASTEXITCODE -ne 0) {
        throw 'Docker integration tests failed.'
    }
}
