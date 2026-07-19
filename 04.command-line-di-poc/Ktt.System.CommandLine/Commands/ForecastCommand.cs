using Ktt.System.CommandLine.Services;
using System.CommandLine;

namespace Ktt.System.CommandLine.Commands;

class ForecastCommand : Command
{
    private readonly WeatherService _weather;

    public ForecastCommand(WeatherService weather) : base("forecast", "Get the forecast. Almost always wrong.")
    {
        _weather = weather;

        var cityOption = new Option<string>("--city")
        {
            Description = "The city.",
            DefaultValueFactory = _ => _weather.Options.DefaultCity
        };
        var daysOption = new Option<int>("--days")
        {
            Description = "Number of days.",
            DefaultValueFactory = _ => _weather.Options.DefaultForecastDays
        };

        Options.Add(cityOption);
        Options.Add(daysOption);

        this.SetAction(async (parseResult, cancellationToken) =>
        {
            var city = parseResult.GetValue(cityOption);
            var days = parseResult.GetValue(daysOption);
            var report = await _weather.Forecast(days, city);
            foreach (var item in report)
            {
                Console.WriteLine(item);
            }
        });
    }
}