using Ktt.KiotaClients.Config;
using Ktt.Ktt.KiotaClients.HttpClients.HttpStatus;
using Ktt.Ktt.KiotaClients.HttpClients.PetStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace Ktt.KiotaClients;

public static class Clients
{
    public static IServiceCollection AddKiotaClients(IServiceCollection services)
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
