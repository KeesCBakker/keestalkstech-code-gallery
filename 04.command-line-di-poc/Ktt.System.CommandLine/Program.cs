using Ktt.System.CommandLine.Commands;
using Ktt.System.CommandLine.Services;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

static void ConfigureServices(IServiceCollection services)
{
    // add commands:
    services.AddTransient<Command, CurrentCommand>();
    services.AddTransient<Command, ForecastCommand>();

    // add services:
    services.AddTransient<WeatherService>();
}

// create service collection
var services = new ServiceCollection();
ConfigureServices(services);

// create service provider
using var serviceProvider = services.BuildServiceProvider();

// entry to run app
var rootCommand = new RootCommand("Weather information using a very unreliable weather service.");
serviceProvider
    .GetServices<Command>()
    .ToList()
    .ForEach(rootCommand.AddCommand);

await rootCommand.InvokeAsync(args);

