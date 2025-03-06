using Ktt.Resilience;
using Ktt.Resilience.KiotaClients;
using Ktt.Resilience.NSwagClients;
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
    services.AddNSwagClients();

    // add app
    services.AddTransient<DemoKiotaPetStore>();
    services.AddTransient<DemoNSwagPetStore>();

    services.AddTransient<DemoRetry>();
    services.AddTransient<DemoKiotaRetry>();
    services.AddTransient<DemoNSwagRetry>();

}

// create service collection
var services = new ServiceCollection();
ConfigureServices(services);

// create service provider
using var serviceProvider = services.BuildServiceProvider();

await serviceProvider.GetRequiredService<DemoKiotaPetStore>().RunAsync();
await serviceProvider.GetRequiredService<DemoNSwagPetStore>().RunAsync();

await serviceProvider.GetRequiredService<DemoRetry>().RunAsync();
await serviceProvider.GetRequiredService<DemoKiotaRetry>().RunAsync();
await serviceProvider.GetRequiredService<DemoNSwagRetry>().RunAsync();
