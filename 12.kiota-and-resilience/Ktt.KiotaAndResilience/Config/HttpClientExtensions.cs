using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;

public static class HttpClientExtensions
{
    public static IHttpStandardResiliencePipelineBuilder AddHttpClientWithResilienceHandler<TClass, TConfig>(
        this IServiceCollection services, string sectionName)
        where TClass : class
        where TConfig : HttpClientOptions
    {
        return services
            // bind the configuration section and validate it on startup
            // we need to bind {name} and {name-standard} to hook up
            // resilience options
            .AddNamedOptions<TConfig>(sectionName)
            // add the HttpClient itself, configure the base URL
            .AddHttpClient<TClass>(sectionName, (serviceProvider, client) =>
            {
                var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<TConfig>>();
                var config = optionsMonitor.Get(sectionName);

                if (config == null)
                {
                    var msg = string.IsNullOrEmpty(sectionName) ?
                        $"Configuration section for '{typeof(TConfig).Name}' is missing." :
                        $"Configuration section {sectionName} for '{typeof(TConfig).Name}' is missing.";

                    throw new InvalidOperationException(msg);
                }

                client.BaseAddress = new Uri(config.BaseUrl);
            })
            // add resilience handler
            .AddStandardResilienceHandler();
    }

    private static IServiceCollection AddNamedOptions<TOptions>(this IServiceCollection services, string sectionName)
        where TOptions : class
    {
        // note: we need to bind {name} and {name-standard} to hook up resilience options
        var sectionNames = new string[] { sectionName, sectionName + "-standard" };

        foreach (var name in sectionNames)
        {
            services
                .AddOptionsWithValidateOnStart<TOptions>(name)
                .BindConfiguration(sectionName);

            services
                .AddOptionsWithValidateOnStart<HttpStandardResilienceOptions>(name)
                .BindConfiguration(sectionName);
        }

        return services;
    }
}
