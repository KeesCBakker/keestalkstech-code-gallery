using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Services.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = true)]
public class ApplicationNameAvailableAttribute : SimpleValidationAttribute
{
    protected override bool IsValidValue(object? value, ValidationContext validationContext)
    {
        if (value is not string name || string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        var exists = validationContext
            .GetRequiredService<ProvisionerService>()
            .Exists(name)
            .Result;

        return !exists;
    }
}
