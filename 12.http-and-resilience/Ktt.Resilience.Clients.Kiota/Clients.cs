using Ktt.Resilience.Clients.Kiota.Config;
using Ktt.Resilience.Clients.Kiota.HttpClients.HttpStatus;
using Ktt.Resilience.Clients.Kiota.HttpClients.PetStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace Ktt.Resilience.Clients.Kiota;

public static class Clients
{
    public static IServiceCollection AddKiotaClients(this IServiceCollection services)
    {
        services.AddKiotaClient<PetStoreClient>("HttpClients:PetStore");

        services.AddKiotaClient<HttpStatusClient>("HttpClients:HttpStatus")
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
