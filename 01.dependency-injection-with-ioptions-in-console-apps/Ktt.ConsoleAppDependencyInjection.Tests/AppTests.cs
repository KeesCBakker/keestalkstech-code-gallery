using Ktt.ConsoleAppDependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ktt.ConsoleAppDependencyInjection.Tests;

public class AppTests
{
    private readonly AppOptions _options;

    public AppTests()
    {
        _options = new AppOptions { Greeting = "Hi {0}!" };
    }

    [Test]
    public async Task Execute_WithNoArgs_UsesDefaultName()
    {
        var app = new App(NullLogger<App>.Instance, _options);

        await app.Execute([]);

        var consoleOutput = TestContext.Current!.GetStandardOutput().TrimEnd();
        await Assert.That(consoleOutput).IsEqualTo("Hi World!");
    }

    [Test]
    public async Task Execute_WithName_WritesGreeting()
    {
        var app = new App(NullLogger<App>.Instance, _options);

        await app.Execute(["Kees"]);

        var consoleOutput = TestContext.Current!.GetStandardOutput().TrimEnd();
        await Assert.That(consoleOutput).IsEqualTo("Hi Kees!");
    }

    [Test]
    public async Task Execute_WithMultipleArgs_UsesFirstName()
    {
        var app = new App(NullLogger<App>.Instance, _options);

        await app.Execute(["Alice", "Bob"]);

        var consoleOutput = TestContext.Current!.GetStandardOutput().TrimEnd();
        await Assert.That(consoleOutput).IsEqualTo("Hi Alice!");
    }
}
