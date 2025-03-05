using Ktt.Resilience.Config;
using Ktt.Resilience.NSwagClients.HttpClients.HttpStatus;
using Ktt.Resilience.NSwagClients.HttpClients.PetStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace Ktt.Resilience.NSwagClients;

public static class Clients
{
    public static IServiceCollection AddKiotaClients(IServiceCollection services)
    {
        services
            .AddTransient<IPetStoreApiClient, PetStoreApiClient>()
            .AddHttpClientWithResilienceHandler<PetStoreApiClient>("HttpClients:PetStore");

        services
            .AddTransient<IHttpStatusApiClient, HttpStatusApiClient>()
            .AddHttpClientWithResilienceHandler<HttpStatusApiClient>("HttpClients:HttpStatusApi")
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
