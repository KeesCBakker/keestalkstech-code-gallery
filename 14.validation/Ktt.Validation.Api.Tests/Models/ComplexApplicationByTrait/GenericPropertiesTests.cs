using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services.Validation;
using Ktt.Validation.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

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

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

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
        request.DockerHubRepo = "repo-one";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.Cpu = "blah";

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

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
        request.DockerHubRepo = "repo-one";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.Cpu = "100m";

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

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
        request.Cpu = "100m";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.DockerHubRepo = "blah";

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

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
        request.Cpu = "100m";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.DockerHubRepo = "repo-one";

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

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
        request.Cpu = "100m";
        request.DockerHubRepo = "repo-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.Environment = "blah";

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);
        errors.ShouldContain("Environment", "Environment must exist.");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Accept_Valid_Environment(ComplexApplicationType type)
    {
        var request = CreateBaseRequest(type);
        request.Cpu = "100m";
        request.DockerHubRepo = "repo-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";
        request.Environment = "server-one";

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

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
        request.Cpu = "100m";
        request.DockerHubRepo = "repo-one";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "blah";

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

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
        request.Cpu = "100m";
        request.DockerHubRepo = "repo-one";
        request.Environment = "server-one";
        request.ImageTag = "12-abcefe";
        request.Ram = "100Mi";

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

        errors.ShouldNotContain("Ram");
    }
}
