using Ktt.JwtSecuredApi.Models;
using Ktt.JwtSecuredApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ktt.JwtSecuredApi.Controllers;

[ApiController]
[Route("api/whoami")]
public class DebugController(IUserNameAccessor userNameAccessor) : ControllerBase
{
    [HttpGet]
    public WhoAmIModel WhoAmI()
    {
        return new WhoAmIModel
        {
            UserName = userNameAccessor.UserName,
            Issuer = userNameAccessor.Issuer
        };
    }
}
