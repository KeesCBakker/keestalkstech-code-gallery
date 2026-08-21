using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services.Validation;
using Ktt.Validation.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Ktt.Validation.Api.Tests.Models.ComplexApplicationByTrait;

[NotInParallel]
[ClassDataSource<TestWebApplicationFactory>(Shared = SharedType.PerClass)]
public class PostfixTests(TestWebApplicationFactory fixture)
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
            Command = "tini /app/start.sh",
            Postfix = string.Empty
        };
    }

    [Test]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Require_NonEmpty_Postfix(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);

        _validator.TryValidate(request, out var errors);

        await errors.ShouldContain("Postfix", "Postfix must not be empty.");
    }

    [Test]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Reject_NonKebabCase_Postfix(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Postfix = "MyService";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldContain("Postfix", "The value must be lower-kebab-case and may not contain the words cron, site or service.");
    }

    [Test]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Reject_ForbiddenWords_In_Postfix(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Postfix = "cron-site-service";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldContain("Postfix", "The value must be lower-kebab-case and may not contain the words cron, site or service.");
    }

    [Test]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Allow_Valid_Postfix(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Postfix = "pinger";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldNotContain("Postfix");
    }

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.CronJob)]
    public async Task Should_Require_Empty_Postfix(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Postfix = string.Empty;

        _validator.TryValidate(request, out var errors);

        await errors.ShouldNotContain("Postfix");
    }

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.CronJob)]
    public async Task Should_Reject_NonEmpty_Postfix(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Postfix = "pinger";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldContain("Postfix", "Postfix must be empty.");
    }
}
