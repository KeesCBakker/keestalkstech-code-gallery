using System.Globalization;
using Microsoft.Extensions.Logging;

namespace Ktt.ConsoleAppDependencyInjection;

public class App(ILogger<App> logger, AppOptions options)
{
    public async Task Execute(string[] args)
    {
        var name = args.Length == 0 ? "World" : args[0];

        logger.LogInformation("Starting...");
        var greeting = string.Format(CultureInfo.CurrentCulture, options.Greeting, name);
        logger.LogDebug("Greeting: {Greeting}", greeting);

        Console.WriteLine(greeting);

        logger.LogInformation("Finished!");

        await Task.CompletedTask;
    }
}
