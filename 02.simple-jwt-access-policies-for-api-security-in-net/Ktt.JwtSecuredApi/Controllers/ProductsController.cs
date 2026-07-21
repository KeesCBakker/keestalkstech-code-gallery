using Microsoft.AspNetCore.Mvc;

namespace Ktt.JwtSecuredApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<string>(200)]
    public IActionResult GetProducts()
    {
        return Ok("Access granted to products.");
    }
}
