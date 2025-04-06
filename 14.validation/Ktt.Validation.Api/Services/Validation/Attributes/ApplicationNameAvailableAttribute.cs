using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Services.Validation.Attributes;

public class ApplicationNameAvailableAttribute : IsNotOneOfValidationAttribute
{
    protected override object[] GetValues(ValidationContext validationContext)
    {
        var service = validationContext.GetRequiredService<ProvisionerService>();
        return service.GetApplicationNames().Result;
    }

    protected override string GetInvalidValueMessage(object? invalidValue)
    {
        return $"{invalidValue} is not a valid or allowed. Application name already exists.";
    }
}
