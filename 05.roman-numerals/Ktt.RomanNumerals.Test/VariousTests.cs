namespace Ktt.RomanNumerals.Test;

public class VariousTests
{
    [Fact]
    public void Blog()
    {
        Assert.Equivalent(1910, RomanNumeral.Parse("MDCCCCX").Number);
        Assert.Equivalent(1910, RomanNumeral.Parse("MCMX").Number);

        Assert.Equivalent("CXVIIII", new RomanNumeral(119).ToString(RomanNumeralNotation.Additive));
        Assert.Equivalent("CXIX", new RomanNumeral(119).ToString());
    }

    [Fact]
    public void Example()
    {
        RomanNumeral I = "I";
        RomanNumeral IV = "IV";

        int a = IV - 1;
        int b = 4 - I;
        int c = IV - "I";
        int d = "IV" - I;
        int e = IV - I;

        Assert.Equivalent(3, a);
        Assert.Equivalent(3, b);
        Assert.Equivalent(3, c);
        Assert.Equivalent(3, d);
        Assert.Equivalent(3, e);

        string f = IV - 1;
        string g = 4 - I;
        string h = IV - "I";
        string i = "IV" - I;
        string j = IV - I;

        Assert.Equivalent("III", f);
        Assert.Equivalent("III", g);
        Assert.Equivalent("III", h);
        Assert.Equivalent("III", i);
        Assert.Equivalent("III", j);

        RomanNumeral k = IV - 1;
        RomanNumeral l = 4 - I;
        RomanNumeral m = IV - "I";
        RomanNumeral n = "IV" - I;
        RomanNumeral o = IV - I;

        Assert.Equivalent(3, k);
        Assert.Equivalent(3, l);
        Assert.Equivalent(3, m);
        Assert.Equivalent(3, n);
        Assert.Equivalent(3, o);
    }
}
