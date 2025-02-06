namespace Ktt.RomanNumerals;

public partial class RomanNumeral
{
    private readonly int _number;

    public int Number => _number;

    public RomanNumeralNotation Notation { get; set; }

    public RomanNumeral(int number, RomanNumeralNotation notation = RomanNumeralNotation.Substractive)
    {
        if (number < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(number), "Number should be positive.");
        }

        _number = number;
        Notation = notation;
    }
}