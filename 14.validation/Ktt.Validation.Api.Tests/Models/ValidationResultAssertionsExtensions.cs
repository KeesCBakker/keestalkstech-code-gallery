using System.ComponentModel.DataAnnotations;
public static class ValidationResultAssertionsExtensions
{
    public static void ShouldContain(
        this IEnumerable<ValidationResult> errors,
        string memberName,
        string? expectedMessage = null)
    {
        var matchFound = Matches(errors, memberName, expectedMessage);
        Assert.True(matchFound, BuildFailureMessage(
            memberName,
            expectedMessage,
            isContainCheck: true,
            errors));
    }

    public static void ShouldNotContain(
        this IEnumerable<ValidationResult> errors,
        string memberName,
        string? expectedMessage = null)
    {
        var matchFound = Matches(errors, memberName, expectedMessage);
        Assert.False(matchFound, BuildFailureMessage(
            memberName,
            expectedMessage,
            isContainCheck: false,
            errors));
    }

    private static bool Matches(IEnumerable<ValidationResult> errors, string memberName, string? expectedMessage) =>
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
