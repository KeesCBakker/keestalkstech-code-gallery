using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;

namespace Ktt.Resilience.Clients.Config;

public static class HttpClientExtensions
{
    public static IHttpStandardResiliencePipelineBuilder AddHttpClientWithResilienceHandler<TClass>(
        this IServiceCollection services,
        string sectionName
    )
        where TClass : class
    {
        return services.AddHttpClientWithResilienceHandler<TClass, HttpClientOptions>(sectionName);
    }

    public static IHttpStandardResiliencePipelineBuilder AddHttpClientWithResilienceHandler<TClass, TConfig>(
        this IServiceCollection services,
        string sectionName)
        where TClass : class
        where TConfig : HttpClientOptions, new()
    {
        var httpClientBuilder = services
            // bind the configuration section
            .AddNamedOptionsForHttpClient<TConfig>(sectionName)
            // add the HttpClient itself, configure the base URL
            .AddHttpClient<TClass>(sectionName, (serviceProvider, client) =>
            {
                var monitor = serviceProvider.GetRequiredService<IOptionsMonitor<TConfig>>();
                var config = monitor.Get(sectionName);
                if (config?.BaseUrl != null)
                {
                    client.BaseAddress = new Uri(config.BaseUrl);
                }
            });

        // add resilience handler
        return httpClientBuilder.AddStandardResilienceHandler();
    }

    public static IHttpStandardResiliencePipelineBuilder AddNamedHttpClientWithResilienceHandler(
        this IServiceCollection services, string httpClientName, string sectionName)
    {
        return services.AddNamedHttpClientWithResilienceHandler<HttpClientOptions>(httpClientName, sectionName);
    }

    public static IHttpStandardResiliencePipelineBuilder AddNamedHttpClientWithResilienceHandler<TConfig>(
        this IServiceCollection services, string httpClientName, string sectionName)
        where TConfig : HttpClientOptions, new()
    {
        var httpClientBuilder = services
            // bind the configuration section
            .AddNamedOptionsForHttpClient<TConfig>(sectionName)
            // add the HttpClient itself, configure the base URL
            .AddHttpClient(httpClientName, (serviceProvider, client) =>
            {
                var monitor = serviceProvider.GetRequiredService<IOptionsMonitor<TConfig>>();
                var config = monitor.Get(sectionName);
                if (config?.BaseUrl != null)
                {
                    client.BaseAddress = new Uri(config.BaseUrl);
                }
            });

        // add resilience handler
        return httpClientBuilder.AddStandardResilienceHandler();
    }

    public static IServiceCollection AddNamedOptionsForHttpClient(this IServiceCollection services, string sectionName)
    {
        return AddNamedOptionsForHttpClient<HttpClientOptions>(services, sectionName);
    }

    public static IServiceCollection AddNamedOptionsForHttpClient<TOptions>(this IServiceCollection services, string sectionName)
        where TOptions : HttpClientOptions, new()
    {
        services
            .AddOptionsWithValidateOnStart<TOptions>(sectionName)
            .BindConfiguration(sectionName);

        // note: we need to bind {name-standard} to hook up resilience options
        services
            .AddOptions<HttpStandardResilienceOptions>(sectionName + "-standard")
            .Configure((HttpStandardResilienceOptions options, IOptionsMonitor<TOptions> monitor) =>
            {
                // get the other options and copy the values into
                // the resilience options:
                var config = monitor.Get(sectionName);
                config.CopyTo(options);
            });

        return services;
    }
}
