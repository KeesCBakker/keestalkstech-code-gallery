using Ktt.Resilience.Clients.Config;
using Ktt.Resilience.Clients.NSwag.HttpClients.HttpStatus;
using Ktt.Resilience.Clients.NSwag.HttpClients.PetStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
namespace Ktt.Resilience.Clients.NSwag;

public static class Clients
{
    public static IHttpStandardResiliencePipelineBuilder AddNSwagClient<TService>(
        this IServiceCollection services,
        string sectionName,
        Func<HttpClient, TService> nswagClientFactory
    )
        where TService : class
    {
        var namedHttpClient = nameof(TService) + "." + sectionName;

        var resilience = services.AddNamedHttpClientWithResilienceHandler(namedHttpClient, sectionName);

        services.AddTransient(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var client = factory.CreateClient(namedHttpClient);

            return nswagClientFactory(client);
        });

        return resilience;
    }

    public static IServiceCollection AddNSwagClients(this IServiceCollection services)
    {
        services.AddNSwagClient<IPetStoreApiClient>("HttpClients:PetStore", httpClient =>
        {
            var client = new PetStoreApiClient(httpClient);
            client.BaseUrl = client.BaseUrl!.ToString();
            return client;
        });

        services
            .AddNSwagClient<IHttpStatusApiClient>("HttpClients:HttpStatus", httpClient =>
                new HttpStatusApiClient(httpClient.BaseAddress!.ToString(), httpClient)
            )
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
