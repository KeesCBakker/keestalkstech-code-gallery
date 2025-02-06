using HandlebarsDotNet;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace Ktt.JsonHandlebars;

public static class JsonHandlebarsDotNet
{
    public class JsonTextEncoder : ITextEncoder
    {
        public void Encode(StringBuilder text, TextWriter target)
        {
            Encode(text.ToString(), target);
        }

        public void Encode(string text, TextWriter target)
        {
            if (string.IsNullOrEmpty(text)) return;
            var encoded = JsonEncodedText.Encode(text)
                .ToString()
                .Replace("\\u0022", "\\\"")  // Replace encoded double quotes
                .Replace("\\u005C", "\\\\"); // Replace encoded backslashes

            target.Write(encoded);
        }

        public void Encode<T>(T text, TextWriter target) where T : IEnumerator<char>
        {
            var str = text?.ToString();
            if (str == null) return;
            Encode(str, target);
        }
    }

    public static IHandlebars Create()
    {
        var handlebars = Handlebars.Create();
        handlebars.Configuration.TextEncoder = new JsonTextEncoder();
        return handlebars;
    }

    public static HandlebarsTemplate<object, object> Compile(string template)
    {
        return Create().Compile(template);
    }

    public static string Parse(string template, object input)
    {
        var t = Compile(template);
        var json = t(input);

        try
        {
            JsonConvert.DeserializeObject<dynamic>(json);
        }
        catch (Exception ex)
        {
            throw new InvalidJsonException(ex, json);
        }

        return json;
    }
}
