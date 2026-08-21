using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services.Validation;
using Ktt.Validation.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Ktt.Validation.Api.Tests.Models;

[NotInParallel]
[ClassDataSource<TestWebApplicationFactory>(Shared = SharedType.PerClass)]
public class ComplexApplicationProvisioningTests(TestWebApplicationFactory fixture)
{
    private readonly IDataAnnotationsValidator _validator =
        fixture.Services
            .GetRequiredService<IDataAnnotationsValidator>();

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJob)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task ValidateGenericProperties(ComplexApplicationType type)
    {
        var request = new ComplexApplication
        {
            Type = type
        };

        // 1. validate required fields
        _validator.TryValidate(request, out var errors);

        await errors.ShouldContain("Name", "The Name field is required.");
        await errors.ShouldContain("Team", "The Team field is required.");
        await errors.ShouldContain("Cpu", "The Cpu field is required.");
        await errors.ShouldContain("Environment", "The Environment field is required.");
        await errors.ShouldContain("DockerHubRepo", "The DockerHubRepo field is required.");
        await errors.ShouldContain("ImageTag", "The ImageTag field is required.");
        await errors.ShouldContain("Ram", "The Ram field is required.");

        // 2. Name
        request.Name = "test";
        _validator.TryValidate(request, out errors);
        await errors.ShouldNotContain("Name");

        // 3. Team

        // 3.1 invalid team
        request.Team = "kaas";
        _validator.TryValidate(request, out errors);
        await errors.ShouldContain("Team");

        // 3.2 valid team
        request.Team = "Racing Greens";
        _validator.TryValidate(request, out errors);
        await errors.ShouldNotContain("Team");

        // 2. CPU

        // 2. invalid CPU
        request.Cpu = "blah";
        _validator.TryValidate(request, out errors);
        await errors.ShouldContain("Cpu", "The field Cpu must match the regular expression '^\\d+m$'.");

        // 2. valid CPU
        request.Cpu = "100m";
        _validator.TryValidate(request, out errors);
        await errors.ShouldNotContain("Cpu");

        // 3. ram

        // 3.1 test invalid ram
        request.Ram = "blah";
        _validator.TryValidate(request, out errors);
        await errors.ShouldContain("Ram", "The field Ram must match the regular expression '^\\d+Mi$'.");

        // 3.2 test valid ram
        request.Ram = "100Mi";
        _validator.TryValidate(request, out errors);
        await errors.ShouldNotContain("Ram");

        // 4. image tag
        request.ImageTag = "12-abcefe";
        _validator.TryValidate(request, out errors);
        await errors.ShouldNotContain("Ram");

        // 5. environment

        // 5.1 invalid environment
        request.Environment = "blah";
        _validator.TryValidate(request, out errors);
        await errors.ShouldContain("Environment");

        // 5.2 valid environment
        request.Environment = "server-one";
        _validator.TryValidate(request, out errors);
        await errors.ShouldNotContain("Environment");

        // 6. dockerhub repo

        // 6.1 invalid dockerhub repo

        request.DockerHubRepo = "blah";
        _validator.TryValidate(request, out errors);
        await errors.ShouldContain("DockerHubRepo");

        // 6.2 valid dockerhub repo
        request.DockerHubRepo = "repo-one";
        _validator.TryValidate(request, out errors);
        await errors.ShouldNotContain("DockerHubRepo");
    }

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    public async Task ValidateApplicationSpecificProperties(ComplexApplicationType type)
    {
        var request = new ComplexApplication
        {
            Name = "test",
            Team = "Racing Greens",
            Type = type,
            Cpu = "100m",
            Ram = "100Mi",
            ImageTag = "12-abcefe",
            Environment = "server-one",
            DockerHubRepo = "repo-one"
        };

        // 1. schedule must be empty
        request.Schedule = "blah";
        _validator.TryValidate(request, out var errors);
        await errors.ShouldContain("Schedule", "Schedule must be empty.");

        // 2. validate schedule empty
        request.Schedule = string.Empty;
        _validator.TryValidate(request, out errors);
        await errors.ShouldNotContain("Schedule");
    }

    [Test]
    [Arguments(ComplexApplicationType.CronJob)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task ValidateCronJobSpecificProperties(ComplexApplicationType type)
    {
        var request = new ComplexApplication
        {
            Name = "test",
            Team = "Racing Greens",
            Type = type,
            Cpu = "100m",
            Ram = "100Mi",
            ImageTag = "12-abcefe",
            Environment = "server-one",
            DockerHubRepo = "repo-one"
        };

        // 1. schedule cannot be empty
        request.Schedule = string.Empty;
        _validator.TryValidate(request, out var errors);
        await errors.ShouldContain("Schedule", "Schedule must not be empty.");

        // 2. invalid schedule
        request.Schedule = "maandag de 14e";
        _validator.TryValidate(request, out errors);
        await errors.ShouldContain("Schedule", "Schedule must be a valid cron expression.");

        // 2. valid schedule
        request.Schedule = "5 4 * * *";
        _validator.TryValidate(request, out errors);
        await errors.ShouldNotContain("Schedule");
    }

    [Test]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task ValidateTypesWithCommand(ComplexApplicationType type)
    {
        var request = new ComplexApplication
        {
            Name = "test",
            Team = "Racing Greens",
            Type = type,
            Cpu = "100m",
            Ram = "100Mi",
            ImageTag = "12-abcefe",
            Environment = "server-one",
            DockerHubRepo = "repo-one"
        };

        // 1. Command and Postfix must not be empty
        request.Command = string.Empty;
        request.Postfix = string.Empty;
        _validator.TryValidate(request, out var errors);
        await errors.ShouldContain("Command", "Command must not be empty.");
        await errors.ShouldContain("Postfix", "Postfix must not be empty.");

        // 2. Scripts without tini is not allowed
        request.Command = "/app/start.sh service-a";
        _validator.TryValidate(request, out errors);
        await errors.ShouldContain("Command", "Script files (.sh) may only be executed when tini is used.");

        // 3. Script with tini is allowed
        request.Command = "tini /app/start.sh service-a";
        _validator.TryValidate(request, out errors);
        await errors.ShouldNotContain("Command");

        // 4. Validate invalid postfix

        // 4.1 Postfix invalid due to casing
        request.Postfix = "I'm a program";
        _validator.TryValidate(request, out errors);
        await errors.ShouldContain("Postfix", "The value must be lower-kebab-case and may not contain the words cron, site or service.");

        // 4.2 Postfix invalid due to forbidden words
        request.Postfix = "cron-site-service";
        _validator.TryValidate(request, out errors);
        await errors.ShouldContain("Postfix", "The value must be lower-kebab-case and may not contain the words cron, site or service.");

        // 4.3 Postfix valid
        request.Postfix = "pinger";
        _validator.TryValidate(request, out errors);
        await errors.ShouldNotContain("Postfix");
    }

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.CronJob)]
    public async Task ValidateTypesWithoutCommand(ComplexApplicationType type)
    {
        var request = new ComplexApplication
        {
            Name = "test",
            Team = "Racing Greens",
            Type = type,
            Cpu = "100m",
            Ram = "100Mi",
            ImageTag = "12-abcefe",
            Environment = "server-one",
            DockerHubRepo = "repo-one"
        };

        // 1. Some fields must be empty
        request.Command = "dotnet run /app/kaas.dll";
        request.Postfix = "kaas";
        _validator.TryValidate(request, out var errors);
        await errors.ShouldContain("Command", "Command must be empty.");
        await errors.ShouldContain("Postfix", "Postfix must be empty.");

        // 2. Command is not allowed
        request.Command = string.Empty;
        _validator.TryValidate(request, out errors);
        await errors.ShouldNotContain("Command");

        // 3. Postfix is not allowed
        request.Postfix = string.Empty;
        _validator.TryValidate(request, out errors);
        await errors.ShouldNotContain("Postfix");
    }
}
