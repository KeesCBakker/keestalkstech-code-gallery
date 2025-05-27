using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ktt.Validation.Api.Controllers;

/// <summary>
/// Controller for provisioning operations.
/// </summary>
[ApiController]
[Route("provision")]
public class ProvisioningController(ProvisionerService service) : ControllerBase
{
    /// <summary>
    /// Provisions a simple application
    /// </summary>
    [HttpPost("simple-application")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public void ProvisisionApplication(SimpleApplication request)
    {
        service.ProvisionApplication(request);
    }

    /// <summary>
    /// Provisions a complex application
    /// </summary>
    [HttpPost("complex-application")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public void ProvisisionApplication(ComplexApplication request)
    {
        service.ProvisionApplication(request);
    }


}
