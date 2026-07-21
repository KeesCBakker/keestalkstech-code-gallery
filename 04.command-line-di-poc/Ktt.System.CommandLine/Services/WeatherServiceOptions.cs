namespace Ktt.System.CommandLine.Services;

sealed class WeatherServiceOptions
{
    public string DefaultCity { get; set; } = "Amsterdam, NLD";

    public int DefaultForecastDays { get; set; } = 5;
}
