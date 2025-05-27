using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Services.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = true)]
public abstract class CustomValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(
        object? value,
        ValidationContext validationContext)
    {
        var valid = IsValidValue(value, validationContext);

        if (!valid)
        {
            return ValidationResult.Success!;
        }

        var msg = GetInvalidValueMessage(value);
        string[] members = validationContext.MemberName is not null
        ? [validationContext.MemberName]
        : [];

        return new ValidationResult(msg, members);
    }

    protected abstract bool IsValidValue(object? value, ValidationContext validationContext);

    protected virtual string GetInvalidValueMessage(object? invalidValue)
    {
        return $"{invalidValue} is not valid or allowed.";
    }
}
