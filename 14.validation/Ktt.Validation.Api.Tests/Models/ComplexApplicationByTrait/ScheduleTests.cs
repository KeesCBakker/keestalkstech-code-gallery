using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services.Validation;
using Ktt.Validation.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Tests.Models.ComplexApplicationProvisioningRequestByTrait;

public class ScheduleTests
{
    private readonly IDataAnnotationsValidator _validator =
        ComplexApplicationProvisioningFixture
            .GetServiceProvider()
            .GetRequiredService<IDataAnnotationsValidator>();

    private ComplexApplication CreateRequest(ComplexApplicationType type)
    {
        return new ComplexApplication
        {
            Type = type,
            Cpu = "100m",
            Ram = "100Mi",
            ImageTag = "12-abcefe",
            Environment = "server-one",
            DockerHubRepo = "repo-one",
            Command = "tini /app/start.sh",
            Postfix = "job-runner"
        };
    }

    [Theory]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Fail_When_Schedule_Is_Empty(ComplexApplicationType type)
    {
        var request = CreateRequest(type);
        request.Schedule = string.Empty;

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

        errors.ShouldContain("Schedule", "Schedule must not be empty.");
    }

    [Theory]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Fail_When_Schedule_Is_Invalid(ComplexApplicationType type)
    {
        var request = CreateRequest(type);
        request.Schedule = "this is not a cron";

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

        errors.ShouldContain("Schedule", "Schedule must be a valid cron expression.");
    }

    [Theory]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Pass_When_Schedule_Is_Valid(ComplexApplicationType type)
    {
        var request = CreateRequest(type);
        request.Schedule = "*/5 * * * *";

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

        errors.ShouldNotContain("Schedule");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    public void Should_Fail_When_Schedule_Is_Provided(ComplexApplicationType type)
    {
        var request = CreateRequest(type);
        request.Schedule = "*/5 * * * *";

        IList<ValidationResult> errors = [];
        _validator.TryValidate(request, out errors);

        errors.ShouldContain("Schedule");
    }
}
