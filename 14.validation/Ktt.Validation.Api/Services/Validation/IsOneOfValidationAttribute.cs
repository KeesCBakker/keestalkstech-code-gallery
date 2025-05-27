using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Services.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = true)]
public abstract class IsOneOfValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(
        object? value,
        ValidationContext validationContext)
    {
        var values = GetValidValues(validationContext);
        var exists = values.Contains(value);

        if (exists)
        {
            return ValidationResult.Success!;
        }

        var msg = GetInvalidValueMessage(value, values);
        string[] members = validationContext.MemberName is not null
            ? [validationContext.MemberName]
            : [];

        return new ValidationResult(msg, members);
    }

    protected abstract object[] GetValidValues(ValidationContext validationContext);

    protected virtual string GetInvalidValueMessage(
        object? invalidValue,
        object[] validValues
    )
    {
        var valid = string.Join(", ", validValues);
        return $"{invalidValue} is not valid or allowed. Options are: [{valid}]";
    }

    protected virtual TOption GetOption<TOption>(ValidationContext validationContext)
        where TOption : class, new()
    {
        return validationContext.GetRequiredService<IOptions<TOption>>().Value;
    }
}
