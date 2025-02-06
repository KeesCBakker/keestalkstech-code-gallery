using System.ComponentModel.DataAnnotations;

public class JwtOptions
{
    public const string SectionName = "JwtSettings";

    [Required(AllowEmptyStrings = false)]
    public string ValidAudience { get; set; } = string.Empty;

    [MinLength(1)]
    public Dictionary<string, string> TrustedServices { get; } = [];

    public Dictionary<string, string[]> AccessPolicies { get; } = [];

    public bool SkipEmptyPublicKeys { get; set; }

    public Dictionary<string, string> GetTrustedServices()
    {
        if (!SkipEmptyPublicKeys)
        {
            return TrustedServices;
        }

        return TrustedServices
            .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}