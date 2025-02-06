using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

public static class JwtAuthenticationExtensions
{
    public static void AddJwtAndAccessPolicies(this IServiceCollection services)
    {
        services
            .AddSingleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerOptionsConfigurator>()
            .AddSingleton<IPostConfigureOptions<AuthorizationOptions>, AuthorizationOptionsConfigurator>()
            .AddSingleton<RsaFactory>()
            .AddAuthorization()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
    }
}