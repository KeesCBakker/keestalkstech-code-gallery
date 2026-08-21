using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services.Validation;
using Ktt.Validation.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Ktt.Validation.Api.Tests.Models.ComplexApplicationByTrait;

[NotInParallel]
[ClassDataSource<TestWebApplicationFactory>(Shared = SharedType.PerClass)]
public class CommandTests(TestWebApplicationFactory fixture)
{
    private readonly IDataAnnotationsValidator _validator =
        fixture.Services
            .GetRequiredService<IDataAnnotationsValidator>();

    private static ComplexApplication CreateDefaultRequestForType(ComplexApplicationType type)
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

    [Test]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Disallow_Script_Command_Without_Tini(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Command = "/app/start.sh";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldContain("Command", "Script files (.sh) may only be executed when tini is used.");
    }

    [Test]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Allow_Script_Command_With_Tini(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Command = "tini /app/start.sh";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldNotContain("Command");
    }

    [Test]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Allow_NonScript_Command_For_CommandTypes(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Command = "dotnet run /app/main.dll";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldNotContain("Command");
    }

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.CronJob)]
    public async Task Should_Not_Allow_Command_For_Types_Without_Command(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Command = "dotnet run /app/service.dll";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldContain("Command", "Command must be empty.");
    }
}
