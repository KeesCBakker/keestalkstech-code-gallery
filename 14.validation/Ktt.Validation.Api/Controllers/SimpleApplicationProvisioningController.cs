using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Ktt.Validation.Api.Controllers;

/// <summary>
/// Controller for provisioning simple applications.
/// </summary>
[ApiController]
[Route("provision/simple-application")]
public class SimpleApplicationProvisioningController(ProvisionerService service) : ControllerBase
{
    /// <summary>
    /// Provisions a simple application
    /// </summary>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public void Provision(SimpleApplication request)
    {
        service.ProvisionApplication(request);
    }

    /// <summary>
    /// Validates a simple application.
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Needed for the validation pipeline.")]
    public void Validate(SimpleApplication request)
    {
        // validation is one by attribute validation
    }

}
