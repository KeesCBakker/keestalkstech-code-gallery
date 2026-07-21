namespace Ktt.RomanNumerals;

public partial class RomanNumeral
{
    public static RomanNumeral operator +(int r1, RomanNumeral r2)
    {
        return new RomanNumeral(r1) + r2;
    }

    public static RomanNumeral operator +(string r1, RomanNumeral r2)
    {
        return Parse(r1) + r2;
    }

    public static RomanNumeral operator +(RomanNumeral r1, string r2)
    {
        return r1 + Parse(r2);
    }

    public static RomanNumeral operator +(RomanNumeral r1, int r2)
    {
        var n = r1.Number + r2;
        return new RomanNumeral(n);
    }

    public static RomanNumeral operator +(RomanNumeral r1, RomanNumeral r2)
    {
        var n = r1.Number + r2.Number;
        return new RomanNumeral(n);
    }

    public static RomanNumeral operator -(int r1, RomanNumeral r2)
    {
        return new RomanNumeral(r1) - r2;
    }

    public static RomanNumeral operator -(string r1, RomanNumeral r2)
    {
        return Parse(r1) - r2;
    }

    // Negative results default to 0 (NULLA) — Romans had no concept of negative numbers
    // See: https://keestalkstech.com/calculations-with-roman-numerals-in-csharp/
    public static RomanNumeral operator -(RomanNumeral r1, RomanNumeral r2)
    {
        var n = r1.Number - r2.Number;

        if (n < 0)
        {
            n = 0;
        }

        return new RomanNumeral(n);
    }
}
