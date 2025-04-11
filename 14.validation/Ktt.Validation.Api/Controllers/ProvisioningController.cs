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
    /// Validates an application provisioning request.
    /// </summary>
    /// <param name="request">The request.</param>
    [HttpPost("application")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public void ProvisisionApplication(ApplicationProvisioningRequest request)
    {
        service.ProvisionApplication(request);
    }

    /// <summary>
    /// Validates an application provisioning request.
    /// </summary>
    /// <param name="request">The request.</param>
    [HttpPost("validate-complex-application")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public void ValidateComplexApplication(ComplexApplication request)
    {
        var i = request.ImageTag;
    }
}
