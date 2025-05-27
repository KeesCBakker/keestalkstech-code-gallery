using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Services.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = true)]
public class EnvironmentAttribute : IsOneOfValidationAttribute
{
    protected override object[] GetValidValues(ValidationContext validationContext) =>
        GetOption<ProvisioningOptions>(validationContext).Environments;
}
