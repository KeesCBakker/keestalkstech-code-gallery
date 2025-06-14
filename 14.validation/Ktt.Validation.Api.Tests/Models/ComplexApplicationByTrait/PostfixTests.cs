﻿using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services.Validation;
using Microsoft.Extensions.DependencyInjection;
using Provisioner.Api.UnitTests;

namespace Ktt.Validation.Api.Tests.Models.ComplexApplicationProvisioningRequestByTrait;

public class PostfixTests
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
            Command = "tini /app/start.sh",
            Postfix = string.Empty
        };
    }

    [Theory]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Require_NonEmpty_Postfix(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);

        _validator.TryValidate(request, out var errors);

        errors.ShouldContain("Postfix", "Postfix must not be empty.");
    }

    [Theory]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Reject_NonKebabCase_Postfix(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Postfix = "MyService";

        _validator.TryValidate(request, out var errors);

        errors.ShouldContain("Postfix", "The value must be lower-kebab-case and may not contain the words cron, site or service.");
    }

    [Theory]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Reject_ForbiddenWords_In_Postfix(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Postfix = "cron-site-service";

        _validator.TryValidate(request, out var errors);

        errors.ShouldContain("Postfix", "The value must be lower-kebab-case and may not contain the words cron, site or service.");
    }

    [Theory]
    [InlineData(ComplexApplicationType.ApplicationWithCommand)]
    [InlineData(ComplexApplicationType.CronJobWithCommand)]
    public void Should_Allow_Valid_Postfix(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Postfix = "pinger";

        _validator.TryValidate(request, out var errors);

        errors.ShouldNotContain("Postfix");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.CronJob)]
    public void Should_Require_Empty_Postfix(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Postfix = string.Empty;

        _validator.TryValidate(request, out var errors);

        errors.ShouldNotContain("Postfix");
    }

    [Theory]
    [InlineData(ComplexApplicationType.Application)]
    [InlineData(ComplexApplicationType.CronJob)]
    public void Should_Reject_NonEmpty_Postfix(ComplexApplicationType type)
    {
        var request = CreateDefaultRequestForType(type);
        request.Postfix = "pinger";

        _validator.TryValidate(request, out var errors);

        errors.ShouldContain("Postfix", "Postfix must be empty.");
    }
}
