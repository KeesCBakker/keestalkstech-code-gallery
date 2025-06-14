using FluentAssertions;
using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services;
using Ktt.Validation.Api.Services.Validation;
using Ktt.Validation.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Provisioner.Api.UnitTests;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace Ktt.Validation.Api.Tests.Models;

public class ApplicationProvisioningRequestTests
{
    [Fact]
    public async Task ValidateByHttpValidation()
    {
        // arrange
        var fixture = new TestWebApplicationFactory();
        var client = fixture.CreateClient();

        // act
        var request = await client.PostAsJsonAsync("/provision/simple-application", new
        {
            name = "My Application",
            type = "Application",
            entryPoint = "dotnet run kaas.is.lekker.dll",
            magicNumber = 1337,
            label = "development"
        });

        request.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        var error = await request.Content.ReadFromJsonAsync<ValidationErrorResponse>();

        // assert
        error.Should().NotBeNull();

        var expectedKeys = new string[] { "EntryPoint", "MagicNumber" };
        expectedKeys.ToList().ForEach(key => error.Errors.Should().ContainKey(key));
    }

    [Fact]
    public void ValidateByService()
    {
        var fixture = new TestWebApplicationFactory();
        var service = fixture.Services.GetRequiredService<ProvisionerService>();

        // arrange
        var request = new SimpleApplication
        {
            Name = "My Application",
            Type = ApplicationType.Application,
            EntryPoint = "dotnet run kaas.is.lekker.dll",
            MagicNumber = 1337,
            Label = "development"
        };

        // act
        var act = () => service.ProvisionApplication(request);

        // assert
        act
            .Should()
            .Throw<ArgumentException>()
            .WithParameterName("request")
            .WithInnerException<System.ComponentModel.DataAnnotations.ValidationException>()
            .WithMessage(
                "Input invalid for '" + nameof(SimpleApplication) + "':\n" +
                "EntryPoint: EntryPoint must be empty.\n" +
                "MagicNumber: Magic number is invalid."
            );
    }

    [Fact]
    public void ValidateByValidator()
    {
        // arrange
        var obj = new SimpleApplication
        {
            Name = "My Application",
            Type = ApplicationType.Application,
            EntryPoint = "dotnet run kaas.is.lekker.dll",
            MagicNumber = 1337,
            Label = "development"
        };

        // act
        IList<ValidationResult> validationErrors = [];
        var context = new ValidationContext(obj);
        var act = () => Validator.TryValidateObject(
            obj,
            context,
            validationErrors,
            true);

        // assert
        act
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("No service for type 'Ktt.Validation.Api.Services.ProvisionerService' has been registered.");
    }

    [Fact]
    public void ValidateByValidatorWithServiceProvider()
    {
        // arrange
        FluentValidationLanguageManager.SetGlobalOptions();

        var provider = new ServiceCollection()
            .AddSingleton<IMagicNumberProvider, MagicNumberProvider>()
            .AddSingleton<IDataAnnotationsValidator, DataAnnotationsValidator>()
            .AddSingleton<ProvisionerService>()
            .AddSingleton(sp => new ProvisioningOptions
            {
                Labels = ["development", "production"]
            })
            .AddTransient(sp => Options.Create(sp.GetRequiredService<ProvisioningOptions>()))
            .AddSingleton(sp => sp)
            .BuildServiceProvider();

        var obj = new SimpleApplication
        {
            Name = "My Application",
            Type = ApplicationType.Application,
            EntryPoint = "dotnet run kaas.is.lekker.dll",
            MagicNumber = 1337,
            Label = "development"
        };

        // act
        IList<ValidationResult> validationErrors = [];
        var context = new ValidationContext(obj, provider, null);
        var valid = Validator.TryValidateObject(obj, context, validationErrors, true);

        // assert
        valid.Should().BeFalse();
        validationErrors.Should().NotBeNullOrEmpty();
        validationErrors.Should().HaveCount(2);

        var messages = validationErrors.Select(e => e.ErrorMessage).ToList();
        messages.Should().Contain("EntryPoint must be empty.");
        messages.Should().Contain("Magic number is invalid.");
    }

    [Fact]
    public void ValidateByDataAnnotationsValidator()
    {
        // arrange
        FluentValidationLanguageManager.SetGlobalOptions();

        var provider = new ServiceCollection()
            .AddSingleton<IMagicNumberProvider, MagicNumberProvider>()
            .AddSingleton<IDataAnnotationsValidator, DataAnnotationsValidator>()
            .AddSingleton<ProvisionerService>()
            .AddSingleton(sp => new ProvisioningOptions
            {
                Labels = ["development", "production"]
            })
            .AddTransient(sp => Options.Create(sp.GetRequiredService<ProvisioningOptions>()))
            .AddSingleton(sp => sp)
            .BuildServiceProvider();

        var obj = new SimpleApplication
        {
            Name = "My Application",
            Type = ApplicationType.Application,
            EntryPoint = "dotnet run kaas.is.lekker.dll",
            MagicNumber = 1337,
            Label = "development"
        };

        // act
        var validator = provider.GetRequiredService<IDataAnnotationsValidator>();
        var valid = validator.TryValidate(obj, out IList<ValidationResult> validationErrors);

        // assert
        valid.Should().BeFalse();
        validationErrors.Should().NotBeNullOrEmpty();
        validationErrors.Should().HaveCount(2);

        var messages = validationErrors.Select(e => e.ErrorMessage).ToList();
        messages.Should().Contain("EntryPoint must be empty.");
        messages.Should().Contain("Magic number is invalid.");
    }
}
