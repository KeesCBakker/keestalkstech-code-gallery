using Ktt.ConsoleAppDependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Threading.Tasks;

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

    [Test]
    public async Task Execute_WithNoArgs_UsesDefaultName()
    {
        var app = new App(_logger, _options);

        await app.Execute([]);

        _logger.Received(1).LogInformation("Starting...");
        _logger.Received(1).LogInformation("Finished!");
    }

    [Test]
    public async Task Execute_WithName_WritesGreeting()
    {
        var app = new App(_logger, _options);

        await app.Execute(["Kees"]);

        var consoleOutput = TestContext.Current!.GetStandardOutput().TrimEnd();
        await Assert.That(consoleOutput).IsEqualTo("Hi Kees!");
    }

    [Test]
    public async Task Execute_WithMultipleArgs_UsesFirstName()
    {
        var app = new App(_logger, _options);

        await app.Execute(["Alice", "Bob"]);

        var consoleOutput = TestContext.Current!.GetStandardOutput().TrimEnd();
        await Assert.That(consoleOutput).IsEqualTo("Hi Alice!");
    }
}
