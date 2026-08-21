using System.ComponentModel.DataAnnotations;
using Ktt.ConsoleAppDependencyInjection;
using System.Threading.Tasks;

namespace Ktt.ConsoleAppDependencyInjection.Tests;

public class AppOptionsTests
{
    [Test]
    public async Task AppOptions_WithValidGreeting_PassesValidation()
    {
        var options = new AppOptions { Greeting = "Hello {0}!" };
        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(options, context, results, validateAllProperties: true);

        await Assert.That(isValid).IsTrue();
        await Assert.That(results).IsEmpty();
    }

    [Test]
    public async Task AppOptions_WithEmptyGreeting_FailsValidation()
    {
        var options = new AppOptions { Greeting = string.Empty };
        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(options, context, results, validateAllProperties: true);

        await Assert.That(isValid).IsFalse();
        await Assert.That(results).Contains(r => r.MemberNames.Contains(nameof(AppOptions.Greeting)));
    }

    [Test]
    public async Task AppOptions_WithWhitespaceGreeting_FailsValidation()
    {
        var options = new AppOptions { Greeting = "   " };
        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(options, context, results, validateAllProperties: true);

        await Assert.That(isValid).IsFalse();
    }

    [Test]
    public async Task AppOptions_HasCorrectSectionName()
    {
        await Assert.That(AppOptions.SectionName).IsEqualTo("App");
    }
}