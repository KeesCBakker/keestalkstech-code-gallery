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
    [NotInParallel]
    public async Task Execute_WithName_WritesGreeting()
    {
        var app = new App(_logger, _options);
        var output = new StringWriter();
        var originalOutput = Console.Out;

        try
        {
#pragma warning disable TUnit0055
            Console.SetOut(output);
#pragma warning restore TUnit0055
            await app.Execute(["Kees"]);
        }
        finally
        {
#pragma warning disable TUnit0055
            Console.SetOut(originalOutput);
#pragma warning restore TUnit0055
        }

        var consoleOutput = output.ToString().TrimEnd();
        await Assert.That(consoleOutput).IsEqualTo("Hi Kees!");
    }

    [Test]
    [NotInParallel]
    public async Task Execute_WithMultipleArgs_UsesFirstName()
    {
        var app = new App(_logger, _options);
        var output = new StringWriter();
        var originalOutput = Console.Out;

        try
        {
#pragma warning disable TUnit0055
            Console.SetOut(output);
#pragma warning restore TUnit0055
            await app.Execute(["Alice", "Bob"]);
        }
        finally
        {
#pragma warning disable TUnit0055
            Console.SetOut(originalOutput);
#pragma warning restore TUnit0055
        }

        var consoleOutput = output.ToString().TrimEnd();
        await Assert.That(consoleOutput).IsEqualTo("Hi Alice!");
    }
}
