using System.ComponentModel.DataAnnotations;

namespace Ktt.ConsoleAppDependencyInjection.Tests;

public class AppOptionsTests
{
    [Fact]
    public void AppOptions_WithValidGreeting_PassesValidation()
    {
        var options = new AppOptions { Greeting = "Hello {0}!" };
        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(options, context, results, validateAllProperties: true);

        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void AppOptions_WithEmptyGreeting_FailsValidation()
    {
        var options = new AppOptions { Greeting = string.Empty };
        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(options, context, results, validateAllProperties: true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(AppOptions.Greeting)));
    }

    [Fact]
    public void AppOptions_WithWhitespaceGreeting_FailsValidation()
    {
        var options = new AppOptions { Greeting = "   " };
        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(options, context, results, validateAllProperties: true);

        Assert.False(isValid);
    }

    [Fact]
    public void AppOptions_HasCorrectSectionName()
    {
        Assert.Equal("App", AppOptions.SectionName);
    }
}
