using Ktt.Resilience.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace Ktt.Resilience.NSwagClients.Config;

public static class NSwagExtensions
{
    public static IHttpStandardResiliencePipelineBuilder AddNSwagClient<TService, TImplementation>(
        this IServiceCollection services,
        string sectionName
    )
            where TService : class
            where TImplementation : class, TService
    {
        var namedHttpClient = nameof(TImplementation) + "." + sectionName;

        var resilience = services.AddNamedHttpClientWithResilienceHandler(namedHttpClient, sectionName);

        services
            .AddTransient(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                var client = factory.CreateClient(namedHttpClient);

                var instance = Activator.CreateInstance(typeof(TImplementation), client);
                return (TService)instance!;
            });

        return resilience;
    }
}
