namespace Ktt.Validation.Api.Services;

public class ProvisioningOptions
{
    public const string SectionName = "Provisioning";

    public string[] Labels { get; set; } = [];

    public string[] Environments { get; set; } = [];

    public string[] Teams { get; set; } = [];
}
