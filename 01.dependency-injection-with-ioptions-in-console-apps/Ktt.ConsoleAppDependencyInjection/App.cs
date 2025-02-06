using Microsoft.Extensions.Logging;

public class App(ILogger<App> _logger, AppOptions _options)
{
    public async Task Execute(string[] args)
    {
        var name = args.Length == 0 ? "World" : args[0];

        _logger.LogInformation("Starting...");
        var greeting = string.Format(_options.Greeting, name);
        _logger.LogDebug("Greeting: {Greeting}", greeting);

        Console.WriteLine(greeting);

        _logger.LogInformation("Finished!");

        await Task.CompletedTask;
    }
}
