namespace Ktt.RomanNumerals;

public partial class RomanNumeral
{
    //subtractive notation uses these numerals
    public static readonly string[] SUBTRACTIVE_NOTATION =
    {
        "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I"
    };

    //the addative notation uses these numerals
    public static readonly string[] ADDITIVE_NOTATION =
    {
        "M", "D", "C", "L", "X", "V", "I"
    };

    public override string ToString()
    {
        return ToString(Notation);
    }

    public string ToString(RomanNumeralNotation notation)
    {
        if (Number == 0)
        {
            return NULLA;
        }

        //check notation for right set of characters
        string[] numerals =
            notation == RomanNumeralNotation.Additive
            ? ADDITIVE_NOTATION
            : SUBTRACTIVE_NOTATION;

        var resultRomanNumeral = "";

        //start with the M and iterate back
        var position = 0;

        //substract till the number is 0
        var value = Number;

        do
        {
            var numeral = numerals[position];
            var numeralValue = VALUES[numeral];

            //check if the value is in the number
            if (value >= numeralValue)
            {
                //substract from the value
                value -= numeralValue;

                //add the numeral to the string
                resultRomanNumeral += numeral;

                //multiple numeral? advance position because things like 'IVIV' does not exist
                bool isMultipleNumeral = numeral.Length > 1;
                if (isMultipleNumeral)
                {
                    position++;
                }

                continue;
            }

            position++;
        }
        while (value != 0);


        return resultRomanNumeral;
    }
}