using System.Threading.Tasks;

namespace Ktt.RomanNumerals.Test;

public class VariousTests
{
    [Test]
    public async Task Blog()
    {
        await Assert.That(RomanNumeral.Parse("MDCCCCX").Number).IsEqualTo(1910);
        await Assert.That(RomanNumeral.Parse("MCMX").Number).IsEqualTo(1910);

        await Assert.That(new RomanNumeral(119).ToString(RomanNumeralNotation.Additive)).IsEqualTo("CXVIIII");
        await Assert.That(new RomanNumeral(119).ToString()).IsEqualTo("CXIX");
    }

    [Test]
    public async Task Example()
    {
        RomanNumeral I = "I";
        RomanNumeral IV = "IV";

        int a = IV - 1;
        int b = 4 - I;
        int c = IV - "I";
        int d = "IV" - I;
        int e = IV - I;

        await Assert.That(a).IsEqualTo(3);
        await Assert.That(b).IsEqualTo(3);
        await Assert.That(c).IsEqualTo(3);
        await Assert.That(d).IsEqualTo(3);
        await Assert.That(e).IsEqualTo(3);

        string f = IV - 1;
        string g = 4 - I;
        string h = IV - "I";
        string i = "IV" - I;
        string j = IV - I;

        await Assert.That(f).IsEqualTo("III");
        await Assert.That(g).IsEqualTo("III");
        await Assert.That(h).IsEqualTo("III");
        await Assert.That(i).IsEqualTo("III");
        await Assert.That(j).IsEqualTo("III");

        RomanNumeral k = IV - 1;
        RomanNumeral l = 4 - I;
        RomanNumeral m = IV - "I";
        RomanNumeral n = "IV" - I;
        RomanNumeral o = IV - I;

        await Assert.That(k.Number).IsEqualTo(3);
        await Assert.That(l.Number).IsEqualTo(3);
        await Assert.That(m.Number).IsEqualTo(3);
        await Assert.That(n.Number).IsEqualTo(3);
        await Assert.That(o.Number).IsEqualTo(3);
    }
}
