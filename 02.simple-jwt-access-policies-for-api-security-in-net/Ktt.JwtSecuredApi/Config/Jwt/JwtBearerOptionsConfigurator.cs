using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class JwtBearerOptionsConfigurator(JwtOptions jwtOptions, RsaFactory rsaFactory) : IPostConfigureOptions<JwtBearerOptions>
{
    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        var trustedServices = jwtOptions.GetTrustedServices();
        if (trustedServices.Count == 0)
        {
            throw new InvalidOperationException("TrustedServices section has configurations errors. No valid service key found.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            // The key of each trusted service is a valid issuer!
            ValidIssuers = trustedServices.Keys,
            ValidateAudience = true,
            ValidAudience = jwtOptions.ValidAudience,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                // Resolve the public key dynamically for the issuer
                if (rsaFactory.TryGetRsa(securityToken.Issuer, out var rsa))
                {
                    return [new RsaSecurityKey(rsa)];
                }

                throw new SecurityTokenInvalidIssuerException("Invalid issuer.");
            }
        };
    }
}
