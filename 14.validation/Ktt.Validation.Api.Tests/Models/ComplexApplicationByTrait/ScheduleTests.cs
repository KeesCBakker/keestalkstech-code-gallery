using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services.Validation;
using Microsoft.Extensions.DependencyInjection;
using Provisioner.Api.UnitTests;

namespace Ktt.Validation.Api.Tests.Models.ComplexApplicationProvisioningRequestByTrait;

public class ScheduleTests
{
    private readonly IDataAnnotationsValidator _validator =
        new TestWebApplicationFactory()
            .Services
            .GetRequiredService<IDataAnnotationsValidator>();

    private ComplexApplication CreateRequest(ComplexApplicationType type)
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

        _validator.TryValidate(request, out var errors);

        errors.ShouldContain("Schedule", "Schedule must not be empty.");
    }

    [Theory]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Fail_When_Schedule_Is_Invalid(ComplexApplicationType type)
    {
        var request = CreateRequest(type);
        request.Schedule = "this is not a cron";

        _validator.TryValidate(request, out var errors);

        errors.ShouldContain("Schedule", "Schedule must be a valid cron expression.");
    }

    [Theory]
    [InlineData(ComplexApplicationType.CronJob)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Pass_When_Schedule_Is_Valid(ComplexApplicationType type)
    {
        var request = CreateRequest(type);
        request.Schedule = "*/5 * * * *";

        _validator.TryValidate(request, out var errors);

        errors.ShouldNotContain("Schedule");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    public void Should_Fail_When_Schedule_Is_Provided(ComplexApplicationType type)
    {
        var request = CreateRequest(type);
        request.Schedule = "*/5 * * * *";

        _validator.TryValidate(request, out var errors);

        errors.ShouldContain("Schedule");
    }
}
