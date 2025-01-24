public static class TimeTravelExtensions
{
    public static DateOnly FirstDayOfMonth(this DateOnly value) => new(value.Year, value.Month, 1);

    public static int DaysInMonth(this DateOnly value) => DateTime.DaysInMonth(value.Year, value.Month);

    public static DateOnly LastDayOfMonth(this DateOnly value) => new(value.Year, value.Month, value.DaysInMonth());

    public static DateOnly FirstDayOfWeek(this DateOnly value) => value.AddDays(-(int)value.DayOfWeek);
}
