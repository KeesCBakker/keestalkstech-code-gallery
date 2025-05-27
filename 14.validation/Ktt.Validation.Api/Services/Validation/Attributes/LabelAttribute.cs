using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Services.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = true)]
public class LabelAttribute : IsOneOfValidationAttribute
{
    protected override object[] GetValues(ValidationContext validationContext)
    {
        var option = GetOption<ProvisioningOptions>(validationContext);
        return option.Labels;
    }

    protected override string GetInvalidValueMessage(
        object? invalidValue,
        object[] validValues)
    {
        var valid = string.Join(", ", validValues);
        return $"{invalidValue} is not a valid or allowed. Options are: [{valid}]";
    }
}
