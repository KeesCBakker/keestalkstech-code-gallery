public class MyDateModel
{
    public DateOnly Stamp { get; set; }

    public DateOnly FirstDayOfTheMonth => Stamp.FirstDayOfMonth();    

    public DateOnly LastDayOfTheMonth => Stamp.LastDayOfMonth();

    public int DaysInMonth => Stamp.DaysInMonth();
}
