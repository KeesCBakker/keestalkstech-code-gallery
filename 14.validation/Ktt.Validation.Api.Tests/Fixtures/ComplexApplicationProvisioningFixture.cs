using Ktt.Validation.Api.Services;
using Ktt.Validation.Api.Services.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Ktt.Validation.Api.Tests.Fixtures;

public static class ComplexApplicationProvisioningFixture
{
    public static IServiceProvider GetServiceProvider()
    {
        FluentValidationLanguageManager.SetGlobalOptions();

        string[] repos = ["repo-one"];

        var dockerHubService = new Mock<IDockerHubService>(MockBehavior.Loose);
        dockerHubService.Setup(x => x.Exists(
            It.IsIn(repos),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(true);

        var services = new ServiceCollection();

        services.AddSingleton(_ => dockerHubService.Object);
        services.AddSingleton<IDataAnnotationsValidator, DataAnnotationsValidator>();

        services.AddSingleton(sp => new ProvisioningOptions
        {
            Labels = ["development", "production"],
            Environments = ["server-one"],
            Teams = ["Red Herrings", "racing-green"]
        })
        .AddTransient(sp => Options.Create(sp.GetRequiredService<ProvisioningOptions>()));

        return services.BuildServiceProvider();
    }
}
