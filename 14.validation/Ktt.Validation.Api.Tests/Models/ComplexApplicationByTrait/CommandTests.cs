using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services.Validation;
using Microsoft.Extensions.DependencyInjection;
using Provisioner.Api.UnitTests;

namespace Ktt.Validation.Api.Tests.Models.ComplexApplicationProvisioningRequestByTrait;

public class CommandTests
{
    private readonly IDataAnnotationsValidator _validator =
        new TestWebApplicationFactory()
            .Services
            .GetRequiredService<IDataAnnotationsValidator>();

    private ComplexApplication CreateDefaultRequestForType(ComplexApplicationType type)
    {
        return new ComplexApplication
        {
            Name = "test",
            Team = "Racing Greens",
            Type = type,
            Cpu = "100m",
            Ram = "100Mi",
            ImageTag = "12-abcefe",
            Environment = "server-one",
            DockerHubRepo = "repo-one",
            Command = string.Empty,
            Postfix = "kafka-processor"
        };
    }

    [Theory]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Disallow_Script_Command_Without_Tini(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Command = "/app/start.sh";

        _validator.TryValidate(request, out var errors);

        errors.ShouldContain("Command", "Script files (.sh) may only be executed when tini is used.");
    }

    [Theory]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Allow_Script_Command_With_Tini(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Command = "tini /app/start.sh";

        _validator.TryValidate(request, out var errors);

        errors.ShouldNotContain("Command");
    }

    [Theory]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Allow_NonScript_Command_For_CommandTypes(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Command = "dotnet run /app/main.dll";

        _validator.TryValidate(request, out var errors);

        errors.ShouldNotContain("Command");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.CronJob)]
    public void Should_Not_Allow_Command_For_Types_Without_Command(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Command = "dotnet run /app/service.dll";

        _validator.TryValidate(request, out var errors);

        errors.ShouldContain("Command", "Command must be empty.");
    }
}
