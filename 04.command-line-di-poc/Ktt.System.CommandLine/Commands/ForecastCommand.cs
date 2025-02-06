using Ktt.System.CommandLine.Services;
using System.CommandLine;

namespace Ktt.System.CommandLine.Commands;

class ForecastCommand : Command
{
    private readonly WeatherService _weather;

    public ForecastCommand(WeatherService weather) : base("forecast", "Get the forecast. Almost always wrong.")
    {
        _weather = weather;

        var cityOption = new Option<string>("--city", ()=> _weather.Options.DefaultCity, "The city.");
        var daysOption = new Option<int>("--days", () => _weather.Options.DefaultForecastDays, "Number of days.");

        AddOption(cityOption);
        AddOption(daysOption);

        this.SetHandler(Execute, cityOption, daysOption);
    }

    private async Task Execute(string city, int days)
    {
        var report = await _weather.Forecast(days, city);
        foreach (var item in report)
        {
            Console.WriteLine(item);
        }
    }
}
