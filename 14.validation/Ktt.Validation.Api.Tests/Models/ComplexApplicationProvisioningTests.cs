using FluentAssertions;
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

        //1. start with nothing -- required fields
        validator.TryValidate(request, out errors);
        errors.Should().Contain("Cpu", "The Cpu field is required.");
        errors.Should().Contain("Environment", "The Environment field is required.");
        errors.Should().Contain("DockerHubRepo", "The DockerHubRepo field is required.");
        errors.Should().Contain("ImageTag", "The ImageTag field is required.");
        errors.Should().Contain("Ram", "The Ram field is required.");

        // 2. cpu

        // 2.1 test invalid cpu
        request.Cpu = "blah";
        validator.TryValidate(request, out errors);
        errors.Should().Contain("Cpu", "The field Cpu must match the regular expression '^\\d+m$'.");

        // 2.2 test valid cpu
        request.Cpu = "100m";
        validator.TryValidate(request, out errors);
        errors.Should().NotContain("Cpu");

        // 3. ram

        // 3.1 test invalid ram
        request.Ram = "blah";
        validator.TryValidate(request, out errors);
        errors.Should().Contain("Ram", "The field Ram must match the regular expression '^\\d+Mi$'.");

        // 3.2 test valid ram
        request.Ram = "100Mi";
        validator.TryValidate(request, out errors);
        errors.Should().NotContain("Ram");

        // 4. image tag
        request.ImageTag = "12-abcefe";
        validator.TryValidate(request, out errors);
        errors.Should().NotContain("Ram");

        // 5. environment
        request.Environment = "blah";
        validator.TryValidate(request, out errors);
        errors.Should().NotContain("Environment");

        // 6. dockerhub repo
        request.DockerHubRepo = "blah";
        validator.TryValidate(request, out errors);
        errors.Should().NotContain("DockerHubRepo", "The DockerHubRepo field is required.");

        // 7. we should still have errors
        errors.Should().Contain("DockerHubRepo", "DockerHub repo must exist.");
        errors.Should().Contain("Environment", "Environment must exist.");

        // 8. valid environment
        request.Environment = "server-one";
        validator.TryValidate(request, out errors);
        errors.Should().NotContain("Environment");

        // 9. valid dockerhub repo
        request.DockerHubRepo = "repo-one";
        validator.TryValidate(request, out errors);
        errors.Should().NotContain("DockerHubRepo");
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

        // 1. schedule may not be provided
        request.Schedule = "blah";
        validator.TryValidate(request, out errors);
        errors.Should().Contain("Schedule", "'Schedule' must be empty.");

        // 2. validate schedule empty
        request.Schedule = string.Empty;
        validator.TryValidate(request, out errors);
        errors.Should().NotContain("Schedule");
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

        // 1. schedule must be provider
        request.Schedule = string.Empty;
        validator.TryValidate(request, out errors);
        errors.Should().Contain("Schedule", "'Schedule' must not be empty.");

        // 2. invalid schedule
        request.Schedule = "maandag de 14e";
        validator.TryValidate(request, out errors);
        errors.Should().Contain("Schedule", "Schedule must be a valid cron expression.");

        // 2. value schedule
        request.Schedule = "5 4 * * *";
        validator.TryValidate(request, out errors);
        errors.Should().NotContain("Schedule");
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

        // 1. Some fields must be provided
        request.Command = string.Empty;
        request.Postfix = string.Empty;
        validator.TryValidate(request, out errors);
        errors.Should().Contain("Command", "'Command' must not be empty.");
        errors.Should().Contain("Postfix", "'Postfix' must not be empty.");

        // 2. Scripts without tini should not be allowed
        request.Command = "/app/start.sh service-a";
        validator.TryValidate(request, out errors);
        errors.Should().Contain("Command", "Script files (.sh) may only be executed when tini is used.");

        // 3. Script with tini should be allowed
        request.Command = "tini /app/start.sh service-a";
        validator.TryValidate(request, out errors);
        errors.Should().NotContain("Command");

        // 4. Validate invalid postfix

        // 4.1 Postfix invalid due to casing
        request.Postfix = "I'm a Service";
        validator.TryValidate(request, out errors);
        errors.Should().Contain("Postfix", "The value must be lower-kebab-case.");

        // 4.2 Postfix invalid due to forbidden words
        request.Postfix = "cron-site-service";
        validator.TryValidate(request, out errors);
        errors.Should().Contain("Postfix", "The value may not contain the words cron, site or service.");

        // 4.3 Postfix valid
        request.Postfix = "pinger";
        validator.TryValidate(request, out errors);
        errors.Should().NotContain("Postfix");
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
        errors.Should().Contain("Command", "'Command' must be empty.");
        errors.Should().Contain("Postfix", "'Postfix' must be empty.");
        
        // 2. Command is not allowed
        request.Command = string.Empty;
        validator.TryValidate(request, out errors);
        errors.Should().NotContain("Command");

        // 3. Postfix is not allowed
        request.Postfix = string.Empty;
        validator.TryValidate(request, out errors);
        errors.Should().NotContain("Postfix");
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
