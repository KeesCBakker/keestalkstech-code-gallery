﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

public static class KiotaExtensions
{
    public static IHttpStandardResiliencePipelineBuilder AddKiotaClient<TClass>(
        this IServiceCollection services,
        string sectionName
    )
        where TClass : class
    {
        var resilienceBuilder = services.AddKiotaHttpClientWithResilienceHandler(sectionName);

        services
            .AddSingleton<AnonymousAuthenticationProvider>()
            .AddTransient(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                var provider = sp.GetRequiredService<AnonymousAuthenticationProvider>();
                var client = factory.CreateClient(sectionName);
                var adapter = new HttpClientRequestAdapter(provider, httpClient: client);
                var instance = Activator.CreateInstance(typeof(TClass), adapter);
                return (TClass)instance!;
            });

        return resilienceBuilder;
    }

    public static IHttpStandardResiliencePipelineBuilder AddKiotaHttpClientWithResilienceHandler(
        this IServiceCollection services,
        string sectionName
        )
    {
        services.AddKiotaHandlers();

        var httpClientBuilder = services
            // bind the configuration section
            .AddNamedOptionsForHttpClient<HttpClientOptions>(sectionName)
            // add the HttpClient itself, configure the base URL
            .AddHttpClient(sectionName, (serviceProvider, client) =>
            {
                var monitor = serviceProvider.GetRequiredService<IOptionsMonitor<HttpClientOptions>>();
                var config = monitor.Get(sectionName);
                if (config?.BaseUrl != null)
                {
                    client.BaseAddress = new Uri(config.BaseUrl);
                }
            });

        var resilienceBuilder = httpClientBuilder.AddStandardResilienceHandler();
        httpClientBuilder.AttachKiotaHandlers();

        return resilienceBuilder;
    }

    public static IServiceCollection AddKiotaHandlers(this IServiceCollection services)
    {
        // Dynamically load the Kiota handlers from the Client Factory
        var kiotaHandlers = KiotaClientFactory.GetDefaultHandlerActivatableTypes();

        // And register them in the DI container
        foreach (var handler in kiotaHandlers)
        {
            services.AddTransient(handler);
        }

        return services;
    }

    public static IHttpClientBuilder AttachKiotaHandlers(this IHttpClientBuilder builder)
    {
        // Dynamically load the Kiota handlers from the Client Factory
        var kiotaHandlers = KiotaClientFactory.GetDefaultHandlerActivatableTypes();
        // And attach them to the http client builder
        foreach (var handler in kiotaHandlers)
        {
            builder.AddHttpMessageHandler((sp) => (DelegatingHandler)sp.GetRequiredService(handler));
        }

        return builder;
    }
}
