using FluentAssertions;
using FluentAssertions.Collections;
using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Tests.Models;

public static class ValidationResultAssertionsExtensions
{
    public static AndConstraint<GenericCollectionAssertions<ValidationResult>> Contain(
        this GenericCollectionAssertions<ValidationResult> assertions,
        string memberName,
        string? errorMessage = null)
    {
        return assertions.Contain(e =>
            (errorMessage == null || e.ErrorMessage == errorMessage) &&
            e.MemberNames.Contains(memberName)
        );
    }

    public static AndConstraint<GenericCollectionAssertions<ValidationResult>> NotContain(
        this GenericCollectionAssertions<ValidationResult> assertions,
        string memberName,
        string? errorMessage = null)
    {
        return assertions.NotContain(e =>
            (errorMessage == null || e.ErrorMessage == errorMessage) &&
            e.MemberNames.Contains(memberName)
        );
    }
}
