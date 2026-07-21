namespace Ktt.RomanNumerals;

public partial class RomanNumeral : IComparable, IComparable<RomanNumeral>, IEquatable<int>, IEquatable<string>
{
    public bool Equals(int other)
    {
        return Number == other;
    }

    public bool Equals(string? other)
    {
        if (other is null)
        {
            return false;
        }

        try
        {
            return Number == Parse(other).Number;
        }
        catch
        {
            return false;
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is int i)
        {
            return Number == i;
        }

        if (obj is string s)
        {
            try
            {
                return Number == Parse(s).Number;
            }
            catch
            {
                return false;
            }
        }

        if (obj is RomanNumeral rn)
        {
            return Number == rn.Number;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return this.Number.GetHashCode();
    }
}
