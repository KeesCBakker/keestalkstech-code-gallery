namespace Ktt.RomanNumerals;

public partial class RomanNumeral
{
    public static bool operator ==(RomanNumeral r1, RomanNumeral r2)
    {
        return Compare(r1, r2) == 0;
    }

    public static bool operator !=(RomanNumeral r1, RomanNumeral r2)
    {
        return Compare(r1, r2) != 0;
    }

    public static bool operator ==(RomanNumeral r1, string r2)
    {
        return r1.Number == Parse(r2).Number;
    }

    public static bool operator !=(RomanNumeral r1, string r2)
    {
        return !(r1 == r2);
    }

    public static bool operator ==(string r1, RomanNumeral r2)
    {
        return r2 == r1;
    }

    public static bool operator !=(string r1, RomanNumeral r2)
    {
        return !(r1 == r2);
    }

    public static bool operator <(RomanNumeral r1, RomanNumeral r2)
    {
        return (Compare(r1, r2) < 0);
    }

    public static bool operator >(RomanNumeral r1, RomanNumeral r2)
    {
        return (Compare(r1, r2) > 0);
    }

    public static bool operator <=(RomanNumeral r1, RomanNumeral r2)
    {
        return (Compare(r1, r2) <= 0);
    }

    public static bool operator >=(RomanNumeral r1, RomanNumeral r2)
    {
        return (Compare(r1, r2) >= 0);
    }
}
