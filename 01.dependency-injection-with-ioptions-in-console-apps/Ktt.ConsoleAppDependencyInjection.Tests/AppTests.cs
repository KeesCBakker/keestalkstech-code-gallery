using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Ktt.ConsoleAppDependencyInjection.Tests;

public class AppTests
{
    private readonly ILogger<App> _logger;
    private readonly AppOptions _options;

    public AppTests()
    {
        _logger = Substitute.For<ILogger<App>>();
        _options = new AppOptions { Greeting = "Hi {0}!" };
    }

    [Fact]
    public async Task Execute_WithNoArgs_UsesDefaultName()
    {
        var app = new App(_logger, _options);

        await app.Execute([]);

        _logger.Received(1).LogInformation("Starting...");
        _logger.Received(1).LogInformation("Finished!");
    }

    [Fact]
    public async Task Execute_WithName_WritesGreeting()
    {
        var app = new App(_logger, _options);
        var output = new StringWriter();
        Console.SetOut(output);

        await app.Execute(["Kees"]);

        var consoleOutput = output.ToString().TrimEnd();
        Assert.Equal("Hi Kees!", consoleOutput);
    }

    [Fact]
    public async Task Execute_WithMultipleArgs_UsesFirstName()
    {
        var app = new App(_logger, _options);
        var output = new StringWriter();
        Console.SetOut(output);

        await app.Execute(["Alice", "Bob"]);

        var consoleOutput = output.ToString().TrimEnd();
        Assert.Equal("Hi Alice!", consoleOutput);
    }
}
