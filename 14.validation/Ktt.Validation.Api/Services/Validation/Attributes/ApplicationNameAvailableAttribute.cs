using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Services.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = true)]
public class ApplicationNameAvailableAttribute : IsNotOneOfValidationAttribute
{
    protected override object[] GetForbiddenValues(ValidationContext validationContext)
    {
        var service = validationContext.GetRequiredService<ProvisionerService>();
        return service.GetApplicationNames().Result;
    }
}
