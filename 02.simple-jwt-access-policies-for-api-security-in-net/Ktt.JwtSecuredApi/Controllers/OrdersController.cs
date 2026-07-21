using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ktt.JwtSecuredApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "orders")]
    [ProducesResponseType<string>(200)]
    public IActionResult GetOrders()
    {
        return Ok("Access granted to orders.");
    }
}
