using System.CommandLine;
using Ktt.System.CommandLine.Commands;
using Ktt.System.CommandLine.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

static void ConfigureServices(IServiceCollection services)
{
  // build configuration from environment variables
  var configuration = new ConfigurationBuilder()
      .AddEnvironmentVariables()
      .Build();

  // configure WeatherServiceOptions from environment variables
  services.Configure<WeatherServiceOptions>(configuration);

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

foreach (var command in serviceProvider.GetServices<Command>())
{
  rootCommand.Subcommands.Add(command);
}

return rootCommand.Parse(args).Invoke();
