using System.Threading.Tasks;

namespace Ktt.RomanNumerals.Test;

public class RomanNumeralMath
{
    [Test]
    public async Task RomanNumeral_Add_Int()
    {
        var x = new RomanNumeral(0) + 4;
        await Assert.That(x.Number).IsEqualTo(4);
    }

    [Test]
    public async Task RomanNumeral_Add_String()
    {
        var x = new RomanNumeral(0) + "IV";
        await Assert.That(x.Number).IsEqualTo(4);
    }

    [Test]
    public async Task RomanNumeral_Add_IntAndString()
    {
        var x = new RomanNumeral(0) + 4 + "IV";
        await Assert.That(x.Number).IsEqualTo(8);
    }

    [Test]
    public async Task RomanNumeral_Add_StringAndInt()
    {
        var x = new RomanNumeral(0) + "IV" + 4;
        await Assert.That(x.Number).IsEqualTo(8);
    }

    [Test]
    public async Task RomanNumeral_Assign_String()
    {
        RomanNumeral? x = "IV";
        await Assert.That(x?.Number).IsEqualTo(4);
    }

    [Test]
    public async Task RomanNumeral_Assign_Int()
    {
        RomanNumeral x = 4;
        await Assert.That(x.Number).IsEqualTo(4);
    }

    [Test]
    public async Task RomanNumeral_NumeralAddString_ToString()
    {
        RomanNumeral? x = "IV";
        string result = x! + "IV";

        await Assert.That(result).IsEqualTo("VIII");
    }
}
