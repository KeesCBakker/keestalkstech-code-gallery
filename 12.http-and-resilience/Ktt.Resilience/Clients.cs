using Ktt.Resilience.Config;
using Ktt.Resilience.HttpClients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace Ktt.Resilience;

public static class Clients
{
    public static IServiceCollection AddNSwagClients(IServiceCollection services)
    {
        // the order matters!
        services.AddTransient<HttpStatusApiService>();
        services
            .AddHttpClientWithResilienceHandler<HttpStatusApiService, HttpClientOptions>("HttpClients:HttpStatusApi")
            .Configure(config =>
            {
                config.Retry.OnRetry = async args =>
                {
                    Console.WriteLine($"Retry {args.AttemptNumber}: Retrying after {args.RetryDelay} due to {args.Outcome.Result?.StatusCode}");
                    await Task.CompletedTask;
                };
            });

        return services;
    }
}
