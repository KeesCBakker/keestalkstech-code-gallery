using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services.Validation;
using Ktt.Validation.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Tests.Models.ComplexApplicationProvisioningRequestByTrait;

public class CommandTests
{
    private readonly IDataAnnotationsValidator _validator =
        ComplexApplicationProvisioningFixture
            .GetServiceProvider()
            .GetRequiredService<IDataAnnotationsValidator>();

    private ComplexApplication CreateDefaultRequestForType(ComplexApplicationType type)
    {
        return new ComplexApplication
        {
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

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

        errors.ShouldContain("Command", "Script files (.sh) may only be executed when tini is used.");
    }

    [Theory]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Allow_Script_Command_With_Tini(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Command = "tini /app/start.sh";

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

        errors.ShouldNotContain("Command");
    }

    [Theory]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Allow_NonScript_Command_For_CommandTypes(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Command = "dotnet run /app/main.dll";

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

        errors.ShouldNotContain("Command");
    }
}
