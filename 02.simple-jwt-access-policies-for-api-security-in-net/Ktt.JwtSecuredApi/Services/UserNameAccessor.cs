public interface IUserNameAccessor
{
    string? UserName { get; }
    string? Issuer { get; }
}

public class UserNameAccessor(IHttpContextAccessor contextAccessor) : IUserNameAccessor
{
    // Retrieve the username claim
    public string? UserName => contextAccessor.HttpContext?.User.FindFirst("userName")?.Value;

    // Retrieve the issuer claim
    public string? Issuer => contextAccessor.HttpContext?.User.FindFirst("iss")?.Value;
}