using Ktt.Resilience.Clients;
using Ktt.Resilience.Clients.Kiota;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

static void ConfigureServices(IServiceCollection services)
{
    // build config
    services.AddSingleton<IConfiguration>(_ =>
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build());

    // add services:
    services.AddClients();
    services.AddKiotaClients();

    // add app
    services.AddTransient<DemoPetStore>();
    services.AddTransient<DemoRetry>();

}

// create service collection
var services = new ServiceCollection();
ConfigureServices(services);

// create service provider
using var serviceProvider = services.BuildServiceProvider();

await serviceProvider.GetRequiredService<DemoPetStore>().RunAsync();
await serviceProvider.GetRequiredService<DemoRetry>().RunAsync();
