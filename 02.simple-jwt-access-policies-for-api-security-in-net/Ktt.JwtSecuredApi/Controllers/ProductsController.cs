using Microsoft.AspNetCore.Mvc;

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
