using FluentAssertions;
using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services;
using Ktt.Validation.Api.Tests;
using Ktt.Validation.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace Ktt.Validata.Api.Tests;

public class ApplicationProvisioningRequestUnitTests
{
    [Fact]
    public async Task ValidateHttpValidation()
    {
        // arrange
        var fixture = new TestServerFixture();

        // act
        var request = await fixture.Client.PostAsJsonAsync("/provision/application", new
        {
            name = "My Application",
            type = "Application",
            entryPoint = "dotnet run kaas.is.lekker.dll",
            magicNumber = 1337
        });

        request.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        var error = await request.Content.ReadFromJsonAsync<ValidationErrorResponse>();

        // assert
        error.Should().NotBeNull();

        var expectedKeys = new string[] { "EntryPoint", "MagicNumber" };
        expectedKeys.ToList().ForEach(key => error.Errors.Should().ContainKey(key));
    }

    [Fact]
    public void ValidateService()
    {
        // arrange
        var fixture = new TestServerFixture();
        var service = fixture.Services.GetRequiredService<ProvisionerService>();
        var request = new ApplicationProvisioningRequest
        {
            Name = "My Application",
            Type = ApplicationType.Application,
            EntryPoint = "dotnet run kaas.is.lekker.dll",
            MagicNumber = 1337
        };

        // act
        var act = ()=> service.ProvisionApplication(request);

        // assert
        act
            .Should()
            .Throw<ArgumentException>()
            .WithParameterName("request")
            .WithInnerException<ValidationException>()
            .WithMessage(
                "Input invalid for 'ApplicationProvisioningRequest':\n" +
                "EntryPoint: 'Entry Point' must be empty.\n" +
                "MagicNumber: Magic number is invalid."
            );
    }
}
