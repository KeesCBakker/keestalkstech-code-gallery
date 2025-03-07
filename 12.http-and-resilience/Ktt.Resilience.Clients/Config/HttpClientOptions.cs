using Microsoft.Extensions.Http.Resilience;
using System.ComponentModel.DataAnnotations;

namespace Ktt.Resilience.Clients.Config;

public class HttpClientOptions : HttpStandardResilienceOptions
{
    [Required]
    [Url]
    [RegularExpression(@"^(http|https)://.*$", ErrorMessage = "The URL must start with http:// or https://")]
    public string BaseUrl { get; set; } = string.Empty;

    public HttpClientOptions()
    {
        Retry.Delay = TimeSpan.FromMilliseconds(100);
    }

    public void CopyTo(HttpStandardResilienceOptions other)
    {
        // I hate that this is needed...

        other.AttemptTimeout = AttemptTimeout;
        other.CircuitBreaker = CircuitBreaker;
        other.RateLimiter = RateLimiter;
        other.Retry = Retry;
        other.TotalRequestTimeout = TotalRequestTimeout;
    }
}
