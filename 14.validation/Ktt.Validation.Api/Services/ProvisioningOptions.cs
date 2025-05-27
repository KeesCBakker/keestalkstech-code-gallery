namespace Ktt.Validation.Api.Services;

public class ProvisioningOptions
{
    public string[] Labels { get; set; }
        = ["development", "test", "production"];

    public string[] Environments { get; set; }
        = ["server-one", "server-two", "server-three"];
}
