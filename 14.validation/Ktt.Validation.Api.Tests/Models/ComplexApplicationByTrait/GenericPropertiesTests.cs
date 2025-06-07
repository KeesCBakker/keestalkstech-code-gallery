using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services.Validation;
using Ktt.Validation.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Ktt.Validation.Api.Tests.Models.ComplexApplicationProvisioningRequestByTrait;

public class GenericTests
{
    private readonly IDataAnnotationsValidator _validator =
        ComplexApplicationProvisioningFixture
            .GetServiceProvider()
            .GetRequiredService<IDataAnnotationsValidator>();

    private ComplexApplication CreateBaseRequest(ComplexApplicationType type) => new()
    {
        Type = type
    };

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Require_Required_Fields(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);

        _validator.TryValidate(request, out var errors);

        errors.ShouldContain("Name", "The Name field is required.");
        errors.ShouldContain("Team", "The Team field is required.");
        errors.ShouldContain("Cpu", "The Cpu field is required.");
        errors.ShouldContain("Environment", "The Environment field is required.");
        errors.ShouldContain("DockerHubRepo", "The DockerHubRepo field is required.");
        errors.ShouldContain("ImageTag", "The ImageTag field is required.");
        errors.ShouldContain("Ram", "The Ram field is required.");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Reject_Invalid_Cpu_Format(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "racing-green";
        request.DockerHubRepo = "repo-one";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.Cpu = "blah";

        _validator.TryValidate(request, out var errors);

        errors.ShouldContain("Cpu", "The field Cpu must match the regular expression '^\\d+m$'.");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Accept_Valid_Cpu_Format(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "racing-green";
        request.DockerHubRepo = "repo-one";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.Cpu = "100m";

        _validator.TryValidate(request, out var errors);

        errors.ShouldNotContain("Cpu");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Reject_Invalid_DockerHubRepo(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "racing-green";
        request.Cpu = "100m";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.DockerHubRepo = "blah";

        _validator.TryValidate(request, out var errors);

        errors.ShouldContain("DockerHubRepo", "The DockerHub repository does not exist.");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Accept_Valid_DockerHubRepo(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "racing-green";
        request.Cpu = "100m";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.DockerHubRepo = "repo-one";

        _validator.TryValidate(request, out var errors);

        errors.ShouldNotContain("DockerHubRepo");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Reject_Invalid_Environment(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "racing-green";
        request.Cpu = "100m";
        request.DockerHubRepo = "repo-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.Environment = "blah";

        _validator.TryValidate(request, out var errors);
        errors.ShouldContain("Environment", "blah is not valid or allowed. Options are: [server-one]");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Accept_Valid_Environment(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "racing-green";
        request.Cpu = "100m";
        request.DockerHubRepo = "repo-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.Environment = "server-one";

        _validator.TryValidate(request, out var errors);

        errors.ShouldNotContain("Environment");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Reject_Invalid_Ram_Format(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "racing-green";
        request.Cpu = "100m";
        request.DockerHubRepo = "repo-one";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "blah";

        _validator.TryValidate(request, out var errors);

        errors.ShouldContain("Ram", "The field Ram must match the regular expression '^\\d+Mi$'.");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Accept_Valid_Ram_Format(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Name = "test";
        request.Team = "racing-green";
        request.Cpu = "100m";
        request.DockerHubRepo = "repo-one";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";

        _validator.TryValidate(request, out var errors);

        errors.ShouldNotContain("Ram");
    }
}
