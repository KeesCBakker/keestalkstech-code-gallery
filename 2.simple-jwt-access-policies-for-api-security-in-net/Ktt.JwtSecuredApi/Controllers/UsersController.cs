using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "users")]
    [ProducesResponseType<string>(200)]
    public IActionResult GetUsers()
    {
        return Ok("Access granted to users.");
    }
}
