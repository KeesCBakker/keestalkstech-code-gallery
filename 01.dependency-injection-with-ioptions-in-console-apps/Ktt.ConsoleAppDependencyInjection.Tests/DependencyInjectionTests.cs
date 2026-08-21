using Ktt.ConsoleAppDependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Ktt.ConsoleAppDependencyInjection.Tests;

public class DependencyInjectionTests
{
    private static ServiceProvider BuildServiceProvider(string? greeting = null)
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });

        var configData = new Dictionary<string, string?>();
        if (greeting is not null)
        {
            configData[$"{AppOptions.SectionName}:{nameof(AppOptions.Greeting)}"] = greeting;
        }

        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(configData);

        services.AddSingleton<IConfiguration>(_ => configBuilder.Build());

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
        services.AddTransient<App>();

        return services.BuildServiceProvider();
    }

    [Test]
    public async Task Services_ResolveApp()
    {
        using var provider = BuildServiceProvider("Hello {0}!");

        var app = provider.GetRequiredService<App>();

        await Assert.That(app).IsNotNull();
    }

    [Test]
    public async Task Services_ResolveAppOptions()
    {
        using var provider = BuildServiceProvider("Hi {0}!");

        var options = provider.GetRequiredService<AppOptions>();

        await Assert.That(options).IsNotNull();
        await Assert.That(options.Greeting).IsEqualTo("Hi {0}!");
    }

    [Test]
    public async Task Services_WithValidConfig_ResolvesCorrectly()
    {
        using var provider = BuildServiceProvider("Test {0}!");

        var app = provider.GetRequiredService<App>();
        var options = provider.GetRequiredService<AppOptions>();
        var logger = provider.GetRequiredService<ILogger<App>>();

        await Assert.That(app).IsNotNull();
        await Assert.That(options).IsNotNull();
        await Assert.That(logger).IsNotNull();
        await Assert.That(options.Greeting).IsEqualTo("Test {0}!");
    }

    [Test]
    public async Task Services_WithMissingGreeting_ThrowsValidationException()
    {
        using var provider = BuildServiceProvider(greeting: null);

        var ex = await Assert.That(() =>
            provider.GetRequiredService<AppOptions>()).Throws<OptionsValidationException>();

        await Assert.That(ex!.Message).Contains(nameof(AppOptions.Greeting));
    }

    [Test]
    public async Task Services_WithMissingGreeting_AppResolutionThrows()
    {
        using var provider = BuildServiceProvider(greeting: null);

        await Assert.That(() =>
            provider.GetRequiredService<App>()).Throws<OptionsValidationException>();
    }
}
