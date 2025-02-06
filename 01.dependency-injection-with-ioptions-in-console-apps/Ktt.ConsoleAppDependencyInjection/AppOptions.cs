using System.ComponentModel.DataAnnotations;

public class AppOptions
{
    public const string SectionName = "App";

    [Required(AllowEmptyStrings = false)]
    public string Greeting { get; set; } = String.Empty;
}