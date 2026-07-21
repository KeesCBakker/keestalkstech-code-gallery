namespace Ktt.RomanNumerals;

public partial class RomanNumeral : IComparable, IComparable<RomanNumeral>
{
    public override bool Equals(object? obj)
    {
        return CompareTo(obj) == 0;
    }

    public override int GetHashCode()
    {
        return this.Number.GetHashCode();
    }
}
