using System.Threading.Tasks;

namespace Ktt.RomanNumerals.Test;

public class ClassicRomanNumeralTests
{
    [Test]
    public async Task RomanNumeral_ToString_FullNotation()
    {
        await Assert.That(new RomanNumeral(1).ToString(RomanNumeralNotation.Additive)).IsEqualTo("I");
        await Assert.That(new RomanNumeral(2).ToString(RomanNumeralNotation.Additive)).IsEqualTo("II");
        await Assert.That(new RomanNumeral(3).ToString(RomanNumeralNotation.Additive)).IsEqualTo("III");
        await Assert.That(new RomanNumeral(4).ToString(RomanNumeralNotation.Additive)).IsEqualTo("IIII");
        await Assert.That(new RomanNumeral(5).ToString(RomanNumeralNotation.Additive)).IsEqualTo("V");
        await Assert.That(new RomanNumeral(6).ToString(RomanNumeralNotation.Additive)).IsEqualTo("VI");
        await Assert.That(new RomanNumeral(7).ToString(RomanNumeralNotation.Additive)).IsEqualTo("VII");
        await Assert.That(new RomanNumeral(8).ToString(RomanNumeralNotation.Additive)).IsEqualTo("VIII");
        await Assert.That(new RomanNumeral(9).ToString(RomanNumeralNotation.Additive)).IsEqualTo("VIIII");
        await Assert.That(new RomanNumeral(10).ToString(RomanNumeralNotation.Additive)).IsEqualTo("X");
        await Assert.That(new RomanNumeral(11).ToString(RomanNumeralNotation.Additive)).IsEqualTo("XI");
        await Assert.That(new RomanNumeral(18).ToString(RomanNumeralNotation.Additive)).IsEqualTo("XVIII");
        await Assert.That(new RomanNumeral(19).ToString(RomanNumeralNotation.Additive)).IsEqualTo("XVIIII");
        await Assert.That(new RomanNumeral(118).ToString(RomanNumeralNotation.Additive)).IsEqualTo("CXVIII");
        await Assert.That(new RomanNumeral(119).ToString(RomanNumeralNotation.Additive)).IsEqualTo("CXVIIII");

        await Assert.That(new RomanNumeral(0).ToString(RomanNumeralNotation.Additive)).IsEqualTo("NULLA");
    }

    [Test]
    public async Task RomanNumeral_ToString_SubtractiveNotation()
    {
        await Assert.That(new RomanNumeral(1).ToString()).IsEqualTo("I");
        await Assert.That(new RomanNumeral(2).ToString()).IsEqualTo("II");
        await Assert.That(new RomanNumeral(3).ToString()).IsEqualTo("III");
        await Assert.That(new RomanNumeral(4).ToString()).IsEqualTo("IV");
        await Assert.That(new RomanNumeral(5).ToString()).IsEqualTo("V");
        await Assert.That(new RomanNumeral(6).ToString()).IsEqualTo("VI");
        await Assert.That(new RomanNumeral(7).ToString()).IsEqualTo("VII");
        await Assert.That(new RomanNumeral(8).ToString()).IsEqualTo("VIII");
        await Assert.That(new RomanNumeral(9).ToString()).IsEqualTo("IX");
        await Assert.That(new RomanNumeral(10).ToString()).IsEqualTo("X");
        await Assert.That(new RomanNumeral(11).ToString()).IsEqualTo("XI");
        await Assert.That(new RomanNumeral(18).ToString()).IsEqualTo("XVIII");
        await Assert.That(new RomanNumeral(19).ToString()).IsEqualTo("XIX");
        await Assert.That(new RomanNumeral(118).ToString()).IsEqualTo("CXVIII");
        await Assert.That(new RomanNumeral(119).ToString()).IsEqualTo("CXIX");

        await Assert.That(new RomanNumeral(0).ToString()).IsEqualTo("NULLA");
    }

    [Test]
    public async Task RomanNumeral_Parse_ClassicNotation()
    {
        await Assert.That(RomanNumeral.Parse("I").Number).IsEqualTo(1);
        await Assert.That(RomanNumeral.Parse("II").Number).IsEqualTo(2);
        await Assert.That(RomanNumeral.Parse("III").Number).IsEqualTo(3);
        await Assert.That(RomanNumeral.Parse("IIII").Number).IsEqualTo(4);
        await Assert.That(RomanNumeral.Parse("V").Number).IsEqualTo(5);
        await Assert.That(RomanNumeral.Parse("VI").Number).IsEqualTo(6);
        await Assert.That(RomanNumeral.Parse("VII").Number).IsEqualTo(7);
        await Assert.That(RomanNumeral.Parse("VIII").Number).IsEqualTo(8);
        await Assert.That(RomanNumeral.Parse("VIIII").Number).IsEqualTo(9);
        await Assert.That(RomanNumeral.Parse("X").Number).IsEqualTo(10);
        await Assert.That(RomanNumeral.Parse("XI").Number).IsEqualTo(11);

        await Assert.That(RomanNumeral.Parse("MDCCCCX").Number).IsEqualTo(1910);
        await Assert.That(RomanNumeral.Parse("MCMX").Number).IsEqualTo(1910);

        await Assert.That(RomanNumeral.Parse("CXVIII").Number).IsEqualTo(118);
        await Assert.That(RomanNumeral.Parse("CIIXX").Number).IsEqualTo(118);
        await Assert.That(RomanNumeral.Parse("CXIIX").Number).IsEqualTo(118);

        await Assert.That(RomanNumeral.Parse("CXVIIII").Number).IsEqualTo(119);
        await Assert.That(RomanNumeral.Parse("CXIX").Number).IsEqualTo(119);

        await Assert.That(RomanNumeral.Parse("NULLA").Number).IsEqualTo(0);
    }

    [Test]
    public async Task RomanNumeral_Parse_SubtractiveNotation()
    {
        await Assert.That(RomanNumeral.Parse("XIV").Number).IsEqualTo(14);
        await Assert.That(RomanNumeral.Parse("XIX").Number).IsEqualTo(19);
    }

    [Test]
    [Arguments("IVL")] //invalid order
    public async Task RomanNumeral_Parse_InvalidClassicNotation(string name)
    {
        var act = () => RomanNumeral.Parse(name);
        var exception = await Assert.That(act).Throws<InvalidCastException>();

        //The thrown exception can be used for even more detailed assertions.
        await Assert.That(exception!.Message).IsEqualTo("The string is not a valid Roman numeral.");
    }
}
