using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Services.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = true)]
public class LabelAttribute : IsOneOfValidationAttributeBase
{
  protected override object[] GetValidValues(ValidationContext validationContext) =>
      GetOption<ProvisioningOptions>(validationContext).Labels;
}
