using Microsoft.Extensions.Http.Resilience;
using System.ComponentModel.DataAnnotations;

public class HttpClientOptions : HttpStandardResilienceOptions
{
    [Required]
    [Url]
    [RegularExpression(@"^(http|https)://.*$", ErrorMessage = "The URL must start with http:// or https://")]
    public string BaseUrl { get; set; } = string.Empty;

    public HttpClientOptions()
    {
        Retry.Delay = TimeSpan.FromMilliseconds(500);
    }
}
