namespace Ktt.Validation.Api.Services;

public class ProvisioningOptions
{
    public const string SectionName = "Provisioning";

    public string[] Labels { get; set; } = ["development", "test", "production" ];

    public string[] Environments { get; set; } = ["server-one", "server-two", "server-three"];

    public string[] Teams { get; set; } = [
      "Blue Blaze",
      "Black Blizzards",
      "Racing Greens",
      "Red Herrings",
      "Red Or Alive",
      "Mellow Yellows"
    ];
}
