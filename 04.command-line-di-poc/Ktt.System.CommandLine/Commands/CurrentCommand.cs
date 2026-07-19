using Ktt.System.CommandLine.Services;
using System.CommandLine;

namespace Ktt.System.CommandLine.Commands;

class CurrentCommand : Command
{
    private readonly WeatherService _weather;

    public CurrentCommand(WeatherService weather) : base("current", "Gets the current temperature.")
    {
        _weather = weather ?? throw new ArgumentNullException(nameof(weather));

        var cityOption = new Option<string>("--city")
        {
            Description = "The city.",
            DefaultValueFactory = _ => _weather.Options.DefaultCity
        };

        Options.Add(cityOption);

        this.SetAction(async (parseResult, cancellationToken) =>
        {
            var city = parseResult.GetValue(cityOption);
            var report = await _weather.GetTemperature(city);
            Console.WriteLine(report);
        });
    }
}