using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services;
using Ktt.Validation.Api.Services.Validation;
using Ktt.Validation.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ktt.Validation.Api.Tests.Models;

[NotInParallel]
public class ApplicationProvisioningRequestTests
{
    [Test]
    public async Task ValidateByHttpValidation()
    {
        // arrange
        using var fixture = new TestWebApplicationFactory();
        using var client = fixture.CreateClient();

        // act
        var request = await client.PostAsJsonAsync("/provision/simple-application", new
        {
            name = "My Application",
            type = "Application",
            entryPoint = "dotnet run kaas.is.lekker.dll",
            magicNumber = 1337,
            label = "development"
        });

        await Assert.That(request.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);

        var error = await request.Content.ReadFromJsonAsync<ValidationErrorResponse>();

        // assert
        await Assert.That(error).IsNotNull();

        var expectedKeys = new string[] { "EntryPoint", "MagicNumber" };
        foreach (var key in expectedKeys)
        {
            await Assert.That(error.Errors).ContainsKey(key);
        }
    }

    [Test]
    public async Task ValidateByService()
    {
        using var fixture = new TestWebApplicationFactory();
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
        var exception = Assert.Throws<ArgumentException>(() => service.ProvisionApplication(request));

        // assert
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception).IsTypeOf<ArgumentException>();
        await Assert.That(exception!.ParamName).IsEqualTo("request");
        await Assert.That(exception.InnerException).IsTypeOf<System.ComponentModel.DataAnnotations.ValidationException>();
        await Assert.That(exception.InnerException!.Message).IsEqualTo(
            "Input invalid for '" + nameof(SimpleApplication) + "':\n" +
            "EntryPoint: EntryPoint must be empty.\n" +
            "MagicNumber: Magic number is invalid.");
    }

    [Test]
    public async Task ValidateByValidator()
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
        var exception = Assert.Throws<InvalidOperationException>(() =>
            Validator.TryValidateObject(obj, context, validationErrors, true));

        // assert
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception).IsTypeOf<InvalidOperationException>();
        await Assert.That(exception!.Message).IsEqualTo(
            "No service for type 'Ktt.Validation.Api.Services.ProvisionerService' has been registered.");
    }

    [Test]
    public async Task ValidateByValidatorWithServiceProvider()
    {
        // arrange
        FluentValidationLanguageManager.SetGlobalOptions();

        using var provider = new ServiceCollection()
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
        await Assert.That(valid).IsFalse();
        await Assert.That(validationErrors).IsNotEmpty();
        await Assert.That(validationErrors).Count().IsEqualTo(2);

        var messages = validationErrors.Select(e => e.ErrorMessage).ToList();
        await Assert.That(messages).Contains("EntryPoint must be empty.");
        await Assert.That(messages).Contains("Magic number is invalid.");
    }

    [Test]
    public async Task ValidateByDataAnnotationsValidator()
    {
        // arrange
        FluentValidationLanguageManager.SetGlobalOptions();

        using var provider = new ServiceCollection()
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
        var valid = validator.TryValidate(obj, out var validationErrors);

        // assert
        await Assert.That(valid).IsFalse();
        await Assert.That(validationErrors).IsNotEmpty();
        await Assert.That(validationErrors).Count().IsEqualTo(2);

        var messages = validationErrors.Select(e => e.ErrorMessage).ToList();
        await Assert.That(messages).Contains("EntryPoint must be empty.");
        await Assert.That(messages).Contains("Magic number is invalid.");
    }
}
