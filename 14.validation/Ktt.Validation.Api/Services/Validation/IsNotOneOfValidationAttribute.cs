using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Services.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = true)]
public abstract class IsNotOneOfValidationAttribute : ValidationAttribute
{
    protected abstract object[] GetForbiddenValues(ValidationContext validationContext);

    protected abstract string GetInvalidValueMessage(
        object? invalidValue,
        object[] validValues);

    protected virtual TOption GetOption<TOption>(ValidationContext validationContext)
        where TOption : class, new()
    {
        return validationContext.GetRequiredService<IOptions<TOption>>().Value;
    }

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
        return new ValidationResult(msg, [validationContext.MemberName!]);
    }
}
