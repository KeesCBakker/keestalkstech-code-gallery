﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ktt.Validation.Api.Tests.Fixtures;

public class TestServerFixture : IDisposable
{
    private readonly TestServer _testServer;
    private readonly IHost _host;

    public TestServerFixture(Action<IServiceCollection>? configureServices = null)
    {
        var webhostBuilder = Host.CreateDefaultBuilder()
             .ConfigureWebHostDefaults(builder =>
             {
                 builder.ConfigureAppConfiguration((builderContext, config) =>
                 {
                     config.AddJsonFile("appsettings.json", false, true);
                 });

                 builder.UseStartup<Startup>();
                 builder.ConfigureServices(ConfigureMockedServices);

                 if (configureServices != null)
                 {
                     builder.ConfigureServices(configureServices);
                 }

                 builder.UseTestServer();
             })
             .UseEnvironment("Production");

        _host = webhostBuilder.Start();
        _testServer = _host.GetTestServer();

        Client = _testServer.CreateClient();
    }

    public HttpClient Client { get; }

    public IServiceProvider Services => _host.Services;

    private void ConfigureMockedServices(IServiceCollection services)
    {
    }

    public static void RemoveService<T>(IServiceCollection services)
    {
        services
            .Where(descriptor => descriptor.ServiceType == typeof(T))
            .ToList()
            .ForEach(d => services.Remove(d));
    }

    public static void RemoveBackgroundService<T>(IServiceCollection services)
    {
        services.Where(descriptor => descriptor.ImplementationType == typeof(T))
            .ToList()
            .ForEach(service => services.Remove(service));
    }

    public void Dispose()
    {
        Client.Dispose();
        _testServer.Dispose();
    }

}
