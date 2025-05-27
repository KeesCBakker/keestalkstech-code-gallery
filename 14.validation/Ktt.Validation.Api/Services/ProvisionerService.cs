using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services.Validation;

namespace Ktt.Validation.Api.Services;

public class ProvisionerService(IDataAnnotationsValidator validator)
{
    public void ProvisionApplication(SimpleApplication request)
    {
        ArgumentNullException.ThrowIfNull(request);
        validator.ThrowIfInvalid(request);

        // continue
    }

    public void ProvisionApplication(ComplexApplication request)
    {
        ArgumentNullException.ThrowIfNull(request);
        validator.ThrowIfInvalid(request);

        // continue
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "Mimics how actual services work.")]
    public Task<string[]> GetApplicationNames()
    {
        return Task.FromResult(new string[]
        {
            "app-name-taken",
            "no-such-app-name",
        });
    }
}
