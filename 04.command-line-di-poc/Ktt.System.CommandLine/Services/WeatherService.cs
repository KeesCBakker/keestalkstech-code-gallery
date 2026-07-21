using System.Globalization;
using Microsoft.Extensions.Options;

namespace Ktt.System.CommandLine.Services;

sealed class WeatherService(IOptions<WeatherServiceOptions> opts)
{
    public WeatherServiceOptions Options { get; } = opts.Value;

    public Task<string> GetTemperature(string? city = null)
    {
        if (city == null)
        {
            city = Options.DefaultCity;
        }

        var report = $"In {city} it is now {Random.Shared.Next(-20, 40)} degrees Celsius.";
        return Task.FromResult(report);
    }

    public Task<string[]> Forecast(int days, string? city = null)
    {
        if (city == null)
        {
            city = Options.DefaultCity;
        }

        var reports = new List<string>
        {
            $"Report for {city} for the next {days} days:"
        };

        for (var i = 0; i < days; i++)
        {
            var date = DateTime.Now.AddDays(i + 1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var report = $"- {date}: {Random.Shared.Next(-20, 40),3} degrees Celsius.";
            reports.Add(report);
        }

        return Task.FromResult(reports.ToArray());
    }
}
