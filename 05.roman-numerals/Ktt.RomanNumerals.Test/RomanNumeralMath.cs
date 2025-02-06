namespace Ktt.RomanNumerals.Test;

public class RomanNumeralMath
{
    [Fact]
    public void RomanNumeral_Add_Int()
    {
        var x = new RomanNumeral(0) + 4;
        Assert.Equivalent(4, x.Number);
    }


    [Fact]
    public void RomanNumeral_Add_String()
    {
        var x = new RomanNumeral(0) + "IV";
        Assert.Equivalent(4, x.Number);
    }


    [Fact]
    public void RomanNumeral_Add_IntAndString()
    {
        var x = new RomanNumeral(0) + 4 + "IV";
        Assert.Equivalent(8, x.Number);
    }


    [Fact]
    public void RomanNumeral_Add_StringAndInt()
    {
        var x = new RomanNumeral(0) + "IV" + 4;
        Assert.Equivalent(8, x.Number);
    }


    [Fact]
    public void RomanNumeral_Assign_String()
    {
        RomanNumeral? x = "IV";
        Assert.Equivalent(4, x?.Number);
    }


    [Fact]
    public void RomanNumeral_Assign_Int()
    {
        RomanNumeral x = 4;
        Assert.Equivalent(4, x.Number);
    }


    [Fact]
    public void RomanNumeral_NumeralAddString_ToString()
    {
        RomanNumeral? x = "IV";
        string result = x! + "IV";

        Assert.Equivalent("VIII", result);
    }
}