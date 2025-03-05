using Ktt.KiotaClients.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

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
    services.AddKiotaClient();
    services.AddNSwagClients();



    // add app
    services.AddTransient<DemoRetry>();
    services.AddTransient<DemoKiotaPetStore>();
    services.AddTransient<DemoKiotaRetry>();
}

// create service collection
var services = new ServiceCollection();
ConfigureServices(services);

// create service provider
using var serviceProvider = services.BuildServiceProvider();

await serviceProvider.GetRequiredService<DemoKiotaRetry>().RunAsync();
await serviceProvider.GetRequiredService<DemoKiotaPetStore>().RunAsync();
await serviceProvider.GetRequiredService<DemoRetry>().RunAsync();
