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
