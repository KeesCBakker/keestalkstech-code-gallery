using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services.Validation;
using Ktt.Validation.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Ktt.Validation.Api.Tests.Models.ComplexApplicationByTrait;

[NotInParallel]
[ClassDataSource<TestWebApplicationFactory>(Shared = SharedType.PerClass)]
public class GenericTests(TestWebApplicationFactory fixture)
{
    private readonly IDataAnnotationsValidator _validator =
        fixture.Services
            .GetRequiredService<IDataAnnotationsValidator>();

    private static ComplexApplication CreateBaseRequest(ComplexApplicationType type) => new()
    {
        Type = type
    };

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJob)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Require_Required_Fields(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);

        _validator.TryValidate(request, out var errors);

        await errors.ShouldContain("Name", "The Name field is required.");
        await errors.ShouldContain("Team", "The Team field is required.");
        await errors.ShouldContain("Cpu", "The Cpu field is required.");
        await errors.ShouldContain("Environment", "The Environment field is required.");
        await errors.ShouldContain("DockerHubRepo", "The DockerHubRepo field is required.");
        await errors.ShouldContain("ImageTag", "The ImageTag field is required.");
        await errors.ShouldContain("Ram", "The Ram field is required.");
    }

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJob)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Reject_Invalid_Cpu_Format(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "Racing Greens";
        request.DockerHubRepo = "repo-one";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.Cpu = "blah";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldContain("Cpu", "The field Cpu must match the regular expression '^\\d+m$'.");
    }

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJob)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Accept_Valid_Cpu_Format(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "Racing Greens";
        request.DockerHubRepo = "repo-one";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.Cpu = "100m";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldNotContain("Cpu");
    }

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJob)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Reject_Invalid_DockerHubRepo(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "Racing Greens";
        request.Cpu = "100m";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.DockerHubRepo = "blah";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldContain("DockerHubRepo", "The DockerHub repository does not exist.");
    }

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJob)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Accept_Valid_DockerHubRepo(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "Racing Greens";
        request.Cpu = "100m";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.DockerHubRepo = "repo-one";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldNotContain("DockerHubRepo");
    }

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJob)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Reject_Invalid_Environment(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "Racing Greens";
        request.Cpu = "100m";
        request.DockerHubRepo = "repo-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.Environment = "blah";

        _validator.TryValidate(request, out var errors);
        await errors.ShouldContain("Environment", "blah is not valid or allowed. Options are: [server-one, server-two, server-three]");
    }

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJob)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Accept_Valid_Environment(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "Racing Greens";
        request.Cpu = "100m";
        request.DockerHubRepo = "repo-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.Environment = "server-one";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldNotContain("Environment");
    }

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJob)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Reject_Invalid_Ram_Format(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "Racing Greens";
        request.Cpu = "100m";
        request.DockerHubRepo = "repo-one";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "blah";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldContain("Ram", "The field Ram must match the regular expression '^\\d+Mi$'.");
    }

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    [Arguments(ComplexApplicationType.CronJob)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Accept_Valid_Ram_Format(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "Racing Greens";
        request.Cpu = "100m";
        request.DockerHubRepo = "repo-one";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldNotContain("Ram");
    }
}
