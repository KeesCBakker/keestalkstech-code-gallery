using Ktt.Validation.Api;
using Ktt.Validation.Api.Tests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Provisioner.Api.UnitTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Action<IServiceCollection>? _overrides;

    public TestServiceOverrides Mocks { get; } = new();

    public TestWebApplicationFactory(Action<IServiceCollection>? overrides = null)
    {
        _overrides = overrides;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: false);
        });

        builder.ConfigureServices(services =>
        {
            // Remove all background services
            foreach (var serviceDescriptor in services.ToArray())
            {
                if (serviceDescriptor.ServiceType.IsGenericType &&
                    serviceDescriptor.ServiceType.GetGenericTypeDefinition() == typeof(IHostedService))
                {
                    services.Remove(serviceDescriptor);
                }
            }

            // remove console logger
            services.RemoveAll<Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider>();

            // replace all loggers with NullLogger
            services.RemoveAll(typeof(Microsoft.Extensions.Logging.ILogger<>));
            services.AddSingleton(typeof(Microsoft.Extensions.Logging.ILogger<>), typeof(Microsoft.Extensions.Logging.Abstractions.NullLogger<>));

            Mocks.Apply(services);

            // Apply the additional overrides if provided
            if (_overrides != null)
            {
                _overrides(services);
            }
        });
    }
}
