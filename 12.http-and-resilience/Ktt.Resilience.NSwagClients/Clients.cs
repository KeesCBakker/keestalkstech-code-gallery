using Ktt.Resilience.Config;
using Ktt.Resilience.NSwagClients.Config;
using Ktt.Resilience.NSwagClients.HttpClients.HttpStatus;
using Ktt.Resilience.NSwagClients.HttpClients.PetStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace Ktt.Resilience.NSwagClients;

public static class Clients
{
    public static IServiceCollection AddNSwagClients(this IServiceCollection services)
    {
        services.AddNSwagClient<IPetStoreApiClient, PetStoreApiClient>("HttpClients:PetStore");

        services
            .AddNSwagClient<IHttpStatusApiClient, HttpStatusApiClient>("HttpClients:HttpStatus")
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
