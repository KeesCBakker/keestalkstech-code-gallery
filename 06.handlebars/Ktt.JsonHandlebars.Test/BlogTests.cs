using System.Threading.Tasks;

namespace Ktt.JsonHandlebars.Test;

public class BlogTests
{
    [Test]
    public async Task BlogExampleOne()
    {
        var source = @"{
  ""title"": ""{{title}}"",
  ""body"": ""{{body}}""
}";

        var template = JsonHandlebarsDotNet.Compile(source);

        var data = new
        {
            title = "My new post",
            body = "This\nis\nmy \"first post\"!"
        };

        var result = template(data);
        var expected = @"{
  ""title"": ""My new post"",
  ""body"": ""This\nis\nmy \""first post\""!""
}";

        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task BlogExampleTwo()
    {
        var source =
    @"
{ 
  ""title"": ""{{title}}"",
  ""author"": ""Kees C. Bakker"",
  ""authorUrl"": ""https://keestalkstech.com/about-me/"",
  ""created"": ""2202-09-17 11:52"",
  ""categories"": [ ""programming"" ],
  ""tags"": [ "".NET"" ""C#"", ""HBS"" ],
  ""body"": ""{{body}}""
}".Replace("\r", "");

        var data = new
        {
            title = "My new post",
            body = "First!"
        };

        var exception = await Assert.That(() => JsonHandlebarsDotNet.Parse(source, data)).Throws<InvalidJsonException>();

        var expects = @"After parsing a value an unexpected character was encountered: "". Path 'tags[0]', line 8, position 19.

06 |   ""created"": ""2202-09-17 11:52"",
07 |   ""categories"": [ ""programming"" ],
08 |   ""tags"": [ "".NET"" ""C#"", ""HBS"" ],
------------------------^
09 |   ""body"": ""First!""
10 | }".Replace("\r", "");

        await Assert.That(exception!.Message).IsEqualTo(expects);
    }
}
