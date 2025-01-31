using Ktt.JwtSecuredApi.Models;
using Microsoft.AspNetCore.Mvc;

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
