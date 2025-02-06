namespace Ktt.RomanNumerals;

public partial class RomanNumeral : IComparable, IComparable<RomanNumeral>
{
    public int CompareTo(RomanNumeral? other)
    {
        if (other is null)
        {
            return 1;
        }

        return Number.CompareTo(other.Number);
    }

    public int CompareTo(object? obj)
    {
        return CompareTo(obj as RomanNumeral);
    }

    protected static int Compare(RomanNumeral r1, RomanNumeral r2)
    {
        if (object.ReferenceEquals(r1, r2))
        {
            return 0;
        }
        if (r1 is null)
        {
            return -1;
        }
        return r1.CompareTo(r2);
    }
}
