namespace Ktt.RomanNumerals;

public partial class RomanNumeral
{
    public static implicit operator int(RomanNumeral r)
    {
        return r.Number;
    }

    public static implicit operator string(RomanNumeral r)
    {
        return r.ToString();
    }

    public static implicit operator RomanNumeral(int r)
    {
        return new RomanNumeral(r);
    }

    public static implicit operator RomanNumeral(string r)
    {
        return Parse(r);
    }
}
