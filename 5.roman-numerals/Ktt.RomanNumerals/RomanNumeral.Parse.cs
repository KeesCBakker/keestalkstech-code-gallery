namespace Ktt.RomanNumerals;

public partial class RomanNumeral
{
    //all the options that are used for parsing, in their order of value
    public static readonly string[] NUMERAL_OPTIONS =
    {
        "M", "CM", "D", "Q", "CD", "P", "G", "C", "XC", "L", "F", "XL", "IIXX", "XIIX", "O", "X", "IX", "V", "IV", "I"
    };

    public static RomanNumeral Parse(string? str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return new RomanNumeral(0);
        }

        //upper case the string
        var strToRead = str.ToUpper();

        //nulla? means nothing 0 wasn't invented yet ;-)
        if (strToRead == NULLA)
        {
            return new RomanNumeral(0);
        }

        //if ends in J -> replace it to I (used in medicine)
        if (strToRead.EndsWith('J'))
        {
            strToRead = string.Concat(strToRead.AsSpan(0, strToRead.Length - 1), "I");
        }

        //if a U is present, assume a V
        strToRead = strToRead.Replace('U', 'V');

        //check simple numbers directly in dictionary
        if (VALUES.ContainsKey(str))
        {
            return new RomanNumeral(VALUES[str]);
        }

        var resultNumber = 0;

        //start with M and iterate through the options
        var numeralOptionPointer = 0;

        //continue to read until the string is empty or the numeral options pointer has exceeded all options
        while (!string.IsNullOrEmpty(strToRead) && numeralOptionPointer < NUMERAL_OPTIONS.Length)
        {
            //select the current numeral
            var numeral = NUMERAL_OPTIONS[numeralOptionPointer];

            //read numeral -> check if the numeral is used, otherwise move on to the next one
            if (!strToRead.StartsWith(numeral))
            {
                numeralOptionPointer++;
                continue;
            }

            //add the vaue of the found numeral
            var value = VALUES[numeral];
            resultNumber += value;

            //remove the letters of the numeral from the string
            strToRead = strToRead.Substring(numeral.Length);


            //short hand like IX? -> move on to the next numeral option
            if (numeral.Length > 1)
            {
                numeralOptionPointer++;
            }
        }

        //whole string is read, return the numeral
        if (string.IsNullOrEmpty(strToRead))
        {
            return new RomanNumeral(resultNumber);
        }

        //string is invalid
        throw new InvalidCastException("The string is not a valid Roman numeral.");
    }

    public static bool IsNumeral(string str)
    {
        try
        {
            Parse(str);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
