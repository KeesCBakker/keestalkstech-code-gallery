using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services.Validation;
using Ktt.Validation.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Ktt.Validation.Api.Tests.Models.ComplexApplicationByTrait;

[NotInParallel]
[ClassDataSource<TestWebApplicationFactory>(Shared = SharedType.PerClass)]
public class ScheduleTests(TestWebApplicationFactory fixture)
{
    private readonly IDataAnnotationsValidator _validator =
        fixture.Services
            .GetRequiredService<IDataAnnotationsValidator>();

    private static ComplexApplication CreateRequest(ComplexApplicationType type)
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

    [Test]
    [Arguments(ComplexApplicationType.CronJob)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Fail_When_Schedule_Is_Empty(ComplexApplicationType type)
    {
        var request = CreateRequest(type);
        request.Schedule = string.Empty;

        _validator.TryValidate(request, out var errors);

        await errors.ShouldContain("Schedule", "Schedule must not be empty.");
    }

    [Test]
    [Arguments(ComplexApplicationType.CronJob)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Fail_When_Schedule_Is_Invalid(ComplexApplicationType type)
    {
        var request = CreateRequest(type);
        request.Schedule = "this is not a cron";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldContain("Schedule", "Schedule must be a valid cron expression.");
    }

    [Test]
    [Arguments(ComplexApplicationType.CronJob)]
    [Arguments(ComplexApplicationType.CronJobWithCommand)]
    public async Task Should_Pass_When_Schedule_Is_Valid(ComplexApplicationType type)
    {
        var request = CreateRequest(type);
        request.Schedule = "*/5 * * * *";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldNotContain("Schedule");
    }

    [Test]
    [Arguments(ComplexApplicationType.Application)]
    [Arguments(ComplexApplicationType.ApplicationWithCommand)]
    public async Task Should_Fail_When_Schedule_Is_Provided(ComplexApplicationType type)
    {
        var request = CreateRequest(type);
        request.Schedule = "*/5 * * * *";

        _validator.TryValidate(request, out var errors);

        await errors.ShouldContain("Schedule");
    }
}
