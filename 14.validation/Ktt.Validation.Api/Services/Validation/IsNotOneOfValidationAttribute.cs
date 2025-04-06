using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Services.Validation;

public abstract class IsNotOneOfValidationAttribute : ValidationAttribute
{
    protected abstract object[] GetValues(ValidationContext validationContext);

    protected abstract string GetInvalidValueMessage(object? invalidValue);

    protected TOption GetOption<TOption>(ValidationContext validationContext)
        where TOption : class, new()
    {
        return validationContext.GetRequiredService<IOptions<TOption>>().Value;
    }

    protected override ValidationResult IsValid(
        object? value,
        ValidationContext validationContext)
    {
        var values = GetValues(validationContext);
        var exists = values.Contains(value);

        if (!exists)
        {
            return ValidationResult.Success!;
        }

        var msg = GetInvalidValueMessage(value);
        return new ValidationResult(msg);
    }
}
