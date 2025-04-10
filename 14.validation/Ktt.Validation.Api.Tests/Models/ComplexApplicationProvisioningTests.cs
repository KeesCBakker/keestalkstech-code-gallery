using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services;
using Ktt.Validation.Api.Services.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Tests.Models;

public class ComplexApplicationProvisioningTests
{
    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void ValidateGenericProperties(ComplexApplicationType type)
    {
        var provider = GetServiceProvider();
        var validator = provider.GetRequiredService<IDataAnnotationsValidator>();

        var request = new ComplexApplicationProvisioningRequest
        {
            Type = type
        };

        IList<ValidationResult> errors = [];

        // 1. validate required fields
        validator.TryValidate(request, out errors);
        errors.ShouldContain("Cpu", "The Cpu field is required.");
        errors.ShouldContain("Environment", "The Environment field is required.");
        errors.ShouldContain("DockerHubRepo", "The DockerHubRepo field is required.");
        errors.ShouldContain("ImageTag", "The ImageTag field is required.");
        errors.ShouldContain("Ram", "The Ram field is required.");

        // 2. CPU

        // 2. invalid CPU
        request.Cpu = "blah";
        validator.TryValidate(request, out errors);
        errors.ShouldContain("Cpu", "The field Cpu must match the regular expression '^\\d+m$'.");

        // 2. valid CPU
        request.Cpu = "100m";
        validator.TryValidate(request, out errors);
        errors.ShouldNotContain("Cpu");

        // 3. ram

        // 3.1 test invalid ram
        request.Ram = "blah";
        validator.TryValidate(request, out errors);
        errors.ShouldContain("Ram", "The field Ram must match the regular expression '^\\d+Mi$'.");

        // 3.2 test valid ram
        request.Ram = "100Mi";
        validator.TryValidate(request, out errors);
        errors.ShouldNotContain("Ram");

        // 4. image tag
        request.ImageTag = "12-abcefe";
        validator.TryValidate(request, out errors);
        errors.ShouldNotContain("Ram");

        // 5. environment
        request.Environment = "blah";
        validator.TryValidate(request, out errors);
        errors.ShouldNotContain("Environment");

        // 6. dockerhub repo
        request.DockerHubRepo = "blah";
        validator.TryValidate(request, out errors);
        errors.ShouldNotContain("DockerHubRepo", "The DockerHubRepo field is required.");

        // 7. we should still have errors
        errors.ShouldContain("DockerHubRepo", "DockerHub repo must exist.");
        errors.ShouldContain("Environment", "Environment must exist.");

        // 8. valid environment
        request.Environment = "server-one";
        validator.TryValidate(request, out errors);
        errors.ShouldNotContain("Environment");

        // 9. valid dockerhub repo
        request.DockerHubRepo = "repo-one";
        validator.TryValidate(request, out errors);
        errors.ShouldNotContain("DockerHubRepo");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    public void ValidateApplicationSpecificProperties(ComplexApplicationType type)
    {
        var provider = GetServiceProvider();
        var validator = provider.GetRequiredService<IDataAnnotationsValidator>();

        var request = new ComplexApplicationProvisioningRequest
        {
            Type = type,
            Cpu = "100m",
            Ram = "100Mi",
            ImageTag = "12-abcefe",
            Environment = "server-one",
            DockerHubRepo = "repo-one"
        };

        IList<ValidationResult> errors = [];

        // 1. schedule must be empty
        request.Schedule = "blah";
        validator.TryValidate(request, out errors);
        errors.ShouldContain("Schedule", "'Schedule' must be empty.");

        // 2. validate schedule empty
        request.Schedule = string.Empty;
        validator.TryValidate(request, out errors);
        errors.ShouldNotContain("Schedule");
    }

    [Theory]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void ValidateCronJobSpecificProperties(ComplexApplicationType type)
    {
        var provider = GetServiceProvider();
        var validator = provider.GetRequiredService<IDataAnnotationsValidator>();

        var request = new ComplexApplicationProvisioningRequest
        {
            Type = type,
            Cpu = "100m",
            Ram = "100Mi",
            ImageTag = "12-abcefe",
            Environment = "server-one",
            DockerHubRepo = "repo-one"
        };

        IList<ValidationResult> errors = [];

        // 1. schedule cannot be empty
        request.Schedule = string.Empty;
        validator.TryValidate(request, out errors);
        errors.ShouldContain("Schedule", "'Schedule' must not be empty.");

        // 2. invalid schedule
        request.Schedule = "maandag de 14e";
        validator.TryValidate(request, out errors);
        errors.ShouldContain("Schedule", "Schedule must be a valid cron expression.");

        // 2. valid schedule
        request.Schedule = "5 4 * * *";
        validator.TryValidate(request, out errors);
        errors.ShouldNotContain("Schedule");
    }

    [Theory]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void ValidateTypesWithCommand(ComplexApplicationType type)
    {
        var provider = GetServiceProvider();
        var validator = provider.GetRequiredService<IDataAnnotationsValidator>();

        var request = new ComplexApplicationProvisioningRequest
        {
            Type = type,
            Cpu = "100m",
            Ram = "100Mi",
            ImageTag = "12-abcefe",
            Environment = "server-one",
            DockerHubRepo = "repo-one"
        };

        IList<ValidationResult> errors = [];

        // 1. Command and Postfix must not be empty
        request.Command = string.Empty;
        request.Postfix = string.Empty;
        validator.TryValidate(request, out errors);
        errors.ShouldContain("Command", "'Command' must not be empty.");
        errors.ShouldContain("Postfix", "'Postfix' must not be empty.");

        // 2. Scripts without tini is not allowed
        request.Command = "/app/start.sh service-a";
        validator.TryValidate(request, out errors);
        errors.ShouldContain("Command", "Script files (.sh) may only be executed when tini is used.");

        // 3. Script with tini is allowed
        request.Command = "tini /app/start.sh service-a";
        validator.TryValidate(request, out errors);
        errors.ShouldNotContain("Command");

        // 4. Validate invalid postfix

        // 4.1 Postfix invalid due to casing
        request.Postfix = "I'm a Service";
        validator.TryValidate(request, out errors);
        errors.ShouldContain("Postfix", "The value must be lower-kebab-case.");

        // 4.2 Postfix invalid due to forbidden words
        request.Postfix = "cron-site-service";
        validator.TryValidate(request, out errors);
        errors.ShouldContain("Postfix", "The value may not contain the words cron, site or service.");

        // 4.3 Postfix valid
        request.Postfix = "pinger";
        validator.TryValidate(request, out errors);
        errors.ShouldNotContain("Postfix");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.CronJob)]
    public void ValidateTypesWithoutCommand(ComplexApplicationType type)
    {
        var provider = GetServiceProvider();
        var validator = provider.GetRequiredService<IDataAnnotationsValidator>();

        var request = new ComplexApplicationProvisioningRequest
        {
            Type = type,
            Cpu = "100m",
            Ram = "100Mi",
            ImageTag = "12-abcefe",
            Environment = "server-one",
            DockerHubRepo = "repo-one"
        };

        IList<ValidationResult> errors = [];

        // 1. Some fields must be empty
        request.Command = "dotnet run /app/kaas.dll";
        request.Postfix = "kaas";
        validator.TryValidate(request, out errors);
        errors.ShouldContain("Command", "'Command' must be empty.");
        errors.ShouldContain("Postfix", "'Postfix' must be empty.");
        
        // 2. Command is not allowed
        request.Command = string.Empty;
        validator.TryValidate(request, out errors);
        errors.ShouldNotContain("Command");

        // 3. Postfix is not allowed
        request.Postfix = string.Empty;
        validator.TryValidate(request, out errors);
        errors.ShouldNotContain("Postfix");
    }

    public static IServiceProvider GetServiceProvider()
    {
        string[] environments = ["server-one"];
        string[] repos = ["repo-one"];

        var dockerHubService = new Mock<IDockerHubService>();
        dockerHubService.Setup(x => x.Exists(
            It.IsIn(environments),
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
