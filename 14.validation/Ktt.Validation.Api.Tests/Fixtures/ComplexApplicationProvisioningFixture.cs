using Ktt.Validation.Api.Services;
using Ktt.Validation.Api.Services.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ktt.Validation.Api.Tests.Fixtures;

public static class ComplexApplicationProvisioningFixture
{
    public static IServiceProvider GetServiceProvider()
    {
        FluentValidationLanguageManager.SetGlobalOptions();

        string[] environments = ["server-one"];
        string[] repos = ["repo-one"];

        var dockerHubService = new Mock<IDockerHubService>();
        dockerHubService.Setup(x => x.Exists(
            It.IsIn(repos),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(true);

        var environmentService = new Mock<IEnvironmentService>();
        environmentService.Setup(x => x.Exists(
            It.IsIn(environments),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(true);

        var services = new ServiceCollection();

        services.AddSingleton(_ => dockerHubService.Object);
        services.AddSingleton(_ => environmentService.Object);
        services.AddSingleton<IDataAnnotationsValidator, DataAnnotationsValidator>();

        return services.BuildServiceProvider();
    }
}
