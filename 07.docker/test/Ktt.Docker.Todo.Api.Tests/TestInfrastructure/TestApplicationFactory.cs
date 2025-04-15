using Ktt.Docker.Todo.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Ktt.Todo.Api.Tests.TestInfrastructure;

public class TestApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(ConfigureServices);
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // Remove the original registration
        RemoveService<ITodoRepository>(services);

        // Replace with a mock or custom implementation
        services.AddSingleton<ITodoRepository, MemoryTodoRepository>();
    }

    protected static void RemoveService<T>(IServiceCollection services)
    {
        services
            .Where(descriptor => descriptor.ServiceType == typeof(T))
            .ToList()
            .ForEach(d => services.Remove(d));
    }

    protected static void RemoveBackgroundService<T>(IServiceCollection services)
    {
        services.Where(descriptor => descriptor.ImplementationType == typeof(T))
            .ToList()
            .ForEach(service => services.Remove(service));
    }
}
