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
                Console.WriteLine($"Retry {args.AttemptNumber}: Retrying after {args.Duration} due to {args.Outcome.Result?.StatusCode}");
                await Task.CompletedTask;
            };
        });

    // add app
    services.AddTransient<App>();
}

// create service collection
var services = new ServiceCollection();
ConfigureServices(services);

// create service provider
using var serviceProvider = services.BuildServiceProvider();

// entry to run app
await serviceProvider.GetRequiredService<App>().RunAsync();
