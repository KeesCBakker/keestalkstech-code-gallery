using Ktt.Validation.Api.Models;
using Ktt.Validation.Api.Services;
using Ktt.Validation.Api.Services.Validation.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Ktt.Validation.Api.Controllers;

/// <summary>
/// Provisioning controller for complex applications.
/// </summary>
[ApiController]
[Route("provision/complex-application")]
public class ComplexApplicationProvisioningController(ProvisionerService service) : ControllerBase
{
    /// <summary>
    /// Provisions a complex application.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public void Provision(ComplexApplication request)
    {
        service.ProvisionApplication(request);
    }

    /// <summary>
    /// Validates a complex application.
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Needed for the validation pipeline.")]
    public void Validate(ComplexApplication request)
    {
        // validation is done by attribute validation
    }

    /// <summary>
    /// Gets all the complex application.
    /// </summary>
    [HttpGet("complex-application")]
    [ProducesResponseType(typeof(ComplexApplication[]), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public ComplexApplication[] GetComplexApplications(
        [FromQuery, Team] string team,
        [FromQuery, Environment] string environment
    )
    {
        return [];
    }

}
