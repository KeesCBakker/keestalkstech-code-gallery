namespace Ktt.RomanNumerals.Test;

public class ClassicRomanNumeralTests
{
    [Fact]
    public void RomanNumeral_ToString_FullNotation()
    {
        Assert.Equivalent("I", new RomanNumeral(1).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("II", new RomanNumeral(2).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("III", new RomanNumeral(3).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("IIII", new RomanNumeral(4).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("V", new RomanNumeral(5).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("VI", new RomanNumeral(6).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("VII", new RomanNumeral(7).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("VIII", new RomanNumeral(8).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("VIIII", new RomanNumeral(9).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("X", new RomanNumeral(10).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("XI", new RomanNumeral(11).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("XVIII", new RomanNumeral(18).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("XVIIII", new RomanNumeral(19).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("CXVIII", new RomanNumeral(118).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("CXVIIII", new RomanNumeral(119).ToString(RomanNumeralNotation.Additive));

        Assert.Equivalent("NULLA", new RomanNumeral(0).ToString(RomanNumeralNotation.Additive));
    }

    [Fact]
    public void RomanNumeral_ToString_SubtractiveNotation()
    {
        Assert.Equivalent("I", new RomanNumeral(1).ToString());
        Assert.Equivalent("II", new RomanNumeral(2).ToString());
        Assert.Equivalent("III", new RomanNumeral(3).ToString());
        Assert.Equivalent("IV", new RomanNumeral(4).ToString());
        Assert.Equivalent("V", new RomanNumeral(5).ToString());
        Assert.Equivalent("VI", new RomanNumeral(6).ToString());
        Assert.Equivalent("VII", new RomanNumeral(7).ToString());
        Assert.Equivalent("VIII", new RomanNumeral(8).ToString());
        Assert.Equivalent("IX", new RomanNumeral(9).ToString());
        Assert.Equivalent("X", new RomanNumeral(10).ToString());
        Assert.Equivalent("XI", new RomanNumeral(11).ToString());
        Assert.Equivalent("XVIII", new RomanNumeral(18).ToString());
        Assert.Equivalent("XIX", new RomanNumeral(19).ToString());
        Assert.Equivalent("CXVIII", new RomanNumeral(118).ToString());
        Assert.Equivalent("CXIX", new RomanNumeral(119).ToString());

        Assert.Equivalent("NULLA", new RomanNumeral(0).ToString());
    }

    [Fact]
    public void RomanNumeral_Parse_ClassicNotation()
    {
        Assert.Equivalent(1, RomanNumeral.Parse("I").Number);
        Assert.Equivalent(2, RomanNumeral.Parse("II").Number);
        Assert.Equivalent(3, RomanNumeral.Parse("III").Number);
        Assert.Equivalent(4, RomanNumeral.Parse("IIII").Number);
        Assert.Equivalent(5, RomanNumeral.Parse("V").Number);
        Assert.Equivalent(6, RomanNumeral.Parse("VI").Number);
        Assert.Equivalent(7, RomanNumeral.Parse("VII").Number);
        Assert.Equivalent(8, RomanNumeral.Parse("VIII").Number);
        Assert.Equivalent(9, RomanNumeral.Parse("VIIII").Number);
        Assert.Equivalent(10, RomanNumeral.Parse("X").Number);
        Assert.Equivalent(11, RomanNumeral.Parse("XI").Number);

        Assert.Equivalent(1910, RomanNumeral.Parse("MDCCCCX").Number);
        Assert.Equivalent(1910, RomanNumeral.Parse("MCMX").Number);

        Assert.Equivalent(118, RomanNumeral.Parse("CXVIII").Number);
        Assert.Equivalent(118, RomanNumeral.Parse("CIIXX").Number);
        Assert.Equivalent(118, RomanNumeral.Parse("CXIIX").Number);

        Assert.Equivalent(119, RomanNumeral.Parse("CXVIIII").Number);
        Assert.Equivalent(119, RomanNumeral.Parse("CXIX").Number);

        Assert.Equivalent(0, RomanNumeral.Parse("NULLA").Number);
    }

    [Fact]
    public void RomanNumeral_Parse_SubtractiveNotation()
    {
        Assert.Equivalent(14, RomanNumeral.Parse("XIV").Number);
        Assert.Equivalent(19, RomanNumeral.Parse("XIX").Number);
    }

    [Theory]
    [InlineData("IVL")] //invalid order
    public void RomanNumeral_Parse_InvalidClassicNotation(string name)
    {
        var act = () => RomanNumeral.Parse(name);
        var exception = Assert.Throws<InvalidCastException>(act);

        //The thrown exception can be used for even more detailed assertions.
        Assert.Equal("The string is not a valid Roman numeral.", exception.Message);
    }
}