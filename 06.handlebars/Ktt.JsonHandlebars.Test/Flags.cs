namespace Ktt.JsonHandlebars.Test;

public class Flags
{
    [Flags]
    enum MenuItems
    {
        None = 0,
        Pizza = 1,
        Fries = 2,
        Pancakes = 4,
        Meatballs = 8,
        Pasta = 16,
        StuffWithP = Pizza | Pancakes | Pasta,
        All = Pizza | Fries | Pancakes | Meatballs | Pasta | StuffWithP
    };

    [Fact]
    public void Test()
    {
        var order = new
        {
            items = MenuItems.Pizza | MenuItems.Pancakes
        };

        string source = @"{ 
    ""order"": [
        {{#each items}}""{{this}}""{{#unless @last}},
        {{/unless}}{{/each}}
    ],
    ""orders"": ""{{items}}""
}".Replace("\r", "");

        var gen = new JsonTemplateGenerator();
        var json = gen.Parse(source, order);

        var expected = @"{ 
    ""order"": [
        ""Pizza"",
        ""Pancakes""
    ],
    ""orders"": ""Pizza, Pancakes""
}".Replace("\r", "");

        Assert.Equivalent(expected, json);
    }

}
