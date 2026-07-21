using Ktt.Resilience.Clients.Config;
using Ktt.Resilience.Clients.HttpClients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace Ktt.Resilience.Clients;

public static class Clients
{
    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        // the order matters!
        services.AddTransient<HttpStatusApiService>();
        services
            .AddHttpClientWithResilienceHandler<HttpStatusApiService>("HttpClients:HttpStatusApi")
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
