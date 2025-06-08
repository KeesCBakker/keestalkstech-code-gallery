using Ktt.Validation.Api.Services;
using Ktt.Validation.Api.Services.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Ktt.Validation.Api.Tests.Fixtures;

public class TestServiceOverrides
{
    public Mock<IDockerHubService> DockerHubService { get; } = new(MockBehavior.Loose);

    public ProvisioningOptions ProvisioningOptions { get; } = new();

    public TestServiceOverrides()
    {
        SetupDockerHubService();
    }

    public void Apply(IServiceCollection services)
    {
        FluentValidationLanguageManager.SetGlobalOptions();

        services
            .AddSingleton(_ => DockerHubService.Object)
            .AddSingleton<IDataAnnotationsValidator, DataAnnotationsValidator>()
            .AddSingleton(_ => ProvisioningOptions)
            .AddTransient(sp => Options.Create(sp.GetRequiredService<ProvisioningOptions>()));
    }

    protected virtual void SetupDockerHubService()
    {
        string[] repos = ["repo-one"];

        DockerHubService
            .Setup(x => x.Exists(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync((string repo, CancellationToken _) =>
            {
                if (repo.StartsWith("ktt/"))
                {
                    return true;
                }

                return repos.Contains(repo);
            });
    }
}
