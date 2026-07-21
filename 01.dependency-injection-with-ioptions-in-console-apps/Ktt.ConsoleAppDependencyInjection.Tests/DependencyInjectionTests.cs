using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

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

    [Fact]
    public void Services_ResolveApp()
    {
        using var provider = BuildServiceProvider("Hello {0}!");

        var app = provider.GetRequiredService<App>();

        Assert.NotNull(app);
    }

    [Fact]
    public void Services_ResolveAppOptions()
    {
        using var provider = BuildServiceProvider("Hi {0}!");

        var options = provider.GetRequiredService<AppOptions>();

        Assert.NotNull(options);
        Assert.Equal("Hi {0}!", options.Greeting);
    }

    [Fact]
    public void Services_WithValidConfig_ResolvesCorrectly()
    {
        using var provider = BuildServiceProvider("Test {0}!");

        var app = provider.GetRequiredService<App>();
        var options = provider.GetRequiredService<AppOptions>();
        var logger = provider.GetRequiredService<ILogger<App>>();

        Assert.NotNull(app);
        Assert.NotNull(options);
        Assert.NotNull(logger);
        Assert.Equal("Test {0}!", options.Greeting);
    }

    [Fact]
    public void Services_WithMissingGreeting_ThrowsValidationException()
    {
        using var provider = BuildServiceProvider(greeting: null);

        var ex = Assert.Throws<OptionsValidationException>(() =>
            provider.GetRequiredService<AppOptions>()
        );

        Assert.Contains(nameof(AppOptions.Greeting), ex.Message);
    }

    [Fact]
    public void Services_WithMissingGreeting_AppResolutionThrows()
    {
        using var provider = BuildServiceProvider(greeting: null);

        Assert.Throws<OptionsValidationException>(() =>
            provider.GetRequiredService<App>()
        );
    }
}
