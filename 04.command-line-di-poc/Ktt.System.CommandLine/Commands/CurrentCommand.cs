using Ktt.System.CommandLine.Services;
using System.CommandLine;

namespace Ktt.System.CommandLine.Commands;

class CurrentCommand : Command
{
    private readonly WeatherService _weather;

    public CurrentCommand(WeatherService weather) : base("current", "Gets the current temperature.")
    {
        _weather = weather ?? throw new ArgumentNullException(nameof(weather));

        var cityOption = new Option<string>("--city", () => _weather.Options.DefaultCity, "The city.");

        AddOption(cityOption);

        this.SetHandler(Execute, cityOption);
    }

    private async Task Execute(string city)
    {
        var report = await _weather.GetTemperature(city);
        Console.WriteLine(report);
    }
}
