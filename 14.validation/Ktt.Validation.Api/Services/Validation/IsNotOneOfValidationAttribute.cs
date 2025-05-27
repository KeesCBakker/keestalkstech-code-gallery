using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Services.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = true)]
public abstract class IsNotOneOfValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(
        object? value,
        ValidationContext validationContext)
    {
        var forbiddenValues = GetForbiddenValues(validationContext);
        var exists = forbiddenValues.Contains(value);

        if (!exists)
        {
            return ValidationResult.Success!;
        }

        var msg = GetInvalidValueMessage(value, forbiddenValues);
        string[] members = validationContext.MemberName is not null
        ? [validationContext.MemberName]
        : [];

        return new ValidationResult(msg, members);
    }

    protected abstract object[] GetForbiddenValues(ValidationContext validationContext);

    protected virtual string GetInvalidValueMessage(object? invalidValue, object[] validValues)
    {
        return $"{invalidValue} is not valid or allowed.";
    }
}
