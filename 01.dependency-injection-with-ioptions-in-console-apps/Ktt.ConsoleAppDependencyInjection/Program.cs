using System.ComponentModel.DataAnnotations;
using Ktt.ConsoleAppDependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var environmentName = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production";

static void ConfigureServices(IServiceCollection services, string environmentName)
{
    // configure logging
    services.AddLogging(builder =>
    {
        builder.AddConsole();
        builder.AddDebug();
    });

    // build config
    services.AddSingleton<IConfiguration>(_ =>
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build());

    void Configure<TConfig>(string sectionName) where TConfig : class
    {
        services
            .AddSingleton(p => p.GetRequiredService<IOptions<TConfig>>().Value)
            .AddOptionsWithValidateOnStart<TConfig>()
            .BindConfiguration(sectionName)
            .Validate(options =>
            {
                var results = new List<ValidationResult>();
                var context = new ValidationContext(options);
                if (!Validator.TryValidateObject(options, context, results, validateAllProperties: true))
                {
                    throw new OptionsValidationException(
                        sectionName,
                        typeof(TConfig),
                        results.Select(r => $"[{sectionName}] {r.ErrorMessage}")
                    );
                }
                return true;
            });
    }

    Configure<AppOptions>(AppOptions.SectionName);

    // add services:
    // services.AddTransient<IMyRespository, MyConcreteRepository>();

    // add app
    services.AddTransient<App>();
}

// create service collection
var services = new ServiceCollection();
ConfigureServices(services, environmentName);

// create service provider
using var serviceProvider = services.BuildServiceProvider();

// entry to run app
await serviceProvider.GetRequiredService<App>().Execute(args);
