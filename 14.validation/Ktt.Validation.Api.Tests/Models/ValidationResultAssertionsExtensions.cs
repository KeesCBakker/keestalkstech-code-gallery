using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Tests.Models;

public static class ValidationResultAssertionsExtensions
{
    public static async Task ShouldBeValid(this IEnumerable<ValidationResult> errors)
    {
        var length = errors.Count();

        var formatted = string.Join("\n", errors.Select(e =>
            $" - {string.Join(", ", e.MemberNames)}: {e.ErrorMessage}"));

        await Assert.That(length).IsEqualTo(0)
            .Because("There should be no validation errors, but found:\n" + formatted);
    }

    public static async Task ShouldContain(
        this IEnumerable<ValidationResult> errors,
        string memberName,
        string? expectedMessage = null)
    {
        var matchFound = Matches(errors, memberName, expectedMessage);
        await Assert.That(matchFound).IsTrue().Because(BuildFailureMessage(
            memberName, expectedMessage, isContainCheck: true, errors));
    }

    public static async Task ShouldNotContain(
        this IEnumerable<ValidationResult> errors,
        string memberName,
        string? expectedMessage = null)
    {
        var matchFound = Matches(errors, memberName, expectedMessage);
        await Assert.That(matchFound).IsFalse().Because(BuildFailureMessage(
            memberName, expectedMessage, isContainCheck: false, errors));
    }

    private static bool Matches(
        IEnumerable<ValidationResult> errors,
        string memberName,
        string? expectedMessage) =>
        errors.Any(e =>
            (expectedMessage == null || e.ErrorMessage == expectedMessage) &&
            e.MemberNames.Contains(memberName)
        );

    private static string BuildFailureMessage(
        string memberName,
        string? expectedMessage,
        bool isContainCheck,
        IEnumerable<ValidationResult> errors)
    {
        var header = isContainCheck
            ? $"Expected a validation error for \"{memberName}\""
            : $"Did not expect a validation error for \"{memberName}\"";

        if (expectedMessage != null)
        {
            header += $" with message \"{expectedMessage}\"";
        }

        var formatted = string.Join("\n", errors.Select(e =>
            $" - {string.Join(", ", e.MemberNames)}: {e.ErrorMessage}"));

        return $"{header}, but found:\n{formatted}";
    }
}
