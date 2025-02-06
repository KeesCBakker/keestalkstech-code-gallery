using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

public class AuthorizationOptionsConfigurator(JwtOptions jwtOptions) : IPostConfigureOptions<AuthorizationOptions>
{
    public void PostConfigure(string? name, AuthorizationOptions options)
    {
        // Global policy: Ensure the username claim is present
        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
            {
                var usernameClaim = context.User.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
                return !string.IsNullOrEmpty(usernameClaim);
            })
            .Build();

        // Configure (optional) Access Policies
        var accessPolicies = jwtOptions.AccessPolicies;
        if (accessPolicies.Count > 0)
        {
            foreach (var policyName in accessPolicies.Keys)
            {
                options.AddPolicy(policyName, policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        // Validate issuer (iss claim)
                        var issuerClaim = context.User.Claims.FirstOrDefault(c => c.Type == "iss")?.Value;

                        // Ensure the issuer is allowed for this policy
                        return accessPolicies[policyName].Contains(issuerClaim);
                    });
                });
            }
        }
    }
}
