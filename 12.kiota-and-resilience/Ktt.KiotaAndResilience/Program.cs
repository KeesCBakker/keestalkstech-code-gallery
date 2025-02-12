using Ktt.KiotaAndResilience.HttpClients.HttpStatus;
using Ktt.KiotaAndResilience.HttpClients.PetStore;
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

    // the order matters!
    services.AddTransient<HttpStatusApiService>();
    services
        .AddHttpClientWithResilienceHandler<HttpStatusApiService, HttpClientOptions>("HttpClients:HttpStatusApi")
        .Configure(config =>
        {
            config.Retry.OnRetry = async args =>
            {
                Console.WriteLine($"Retry {args.AttemptNumber}: Retrying after {args.RetryDelay} due to {args.Outcome.Result?.StatusCode}");
                await Task.CompletedTask;
            };
        });

    services.AddKiotaClient<PetStoreClient>("HttpClients:PetStore");

    services.AddKiotaClient<HttpStatusClient>("HttpClients:HttpStatus")
        .Configure(config =>
        {
            config.Retry.OnRetry = async args =>
            {
                Console.WriteLine($"Retry {args.AttemptNumber}: Retrying after {args.RetryDelay} due to {args.Outcome.Result?.StatusCode}");
                await Task.CompletedTask;
            };
        });

    // add app
    services.AddTransient<DemoRetry>();
    services.AddTransient<DemoPetStore>();
    services.AddTransient<DemoKiotaRetry>();
}

// create service collection
var services = new ServiceCollection();
ConfigureServices(services);

// create service provider
using var serviceProvider = services.BuildServiceProvider();

await serviceProvider.GetRequiredService<DemoKiotaRetry>().RunAsync();
await serviceProvider.GetRequiredService<DemoPetStore>().RunAsync();
await serviceProvider.GetRequiredService<DemoRetry>().RunAsync();
