using System.ComponentModel.DataAnnotations;

namespace Ktt.ConsoleAppDependencyInjection;

public class AppOptions
{
    public const string SectionName = "App";

    [Required(AllowEmptyStrings = false)]
    public string Greeting { get; set; } = String.Empty;
}
