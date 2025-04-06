using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services.Validation;

namespace Ktt.Validation.Api.Services;

public class ProvisionerService(IDataAnnotationsValidator validator)
{
    public void ProvisionApplication(ApplicationProvisioningRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        validator.ThrowIfInvalid(request, nameof(request));

        // continue
    }
}
