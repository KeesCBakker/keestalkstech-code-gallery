using HandlebarsDotNet;
using Newtonsoft.Json;
using static Ktt.JsonHandlebars.JsonHandlebarsDotNet;

namespace Ktt.JsonHandlebars;

public class JsonTemplateGenerator : IJsonTemplateGenerator
{
    public IHandlebars Handlebars { get; }

    public JsonTemplateGenerator()
    {
        Handlebars = HandlebarsDotNet.Handlebars.Create();
        Handlebars.Configuration.TextEncoder = new JsonTextEncoder();
        Handlebars.Configuration.ObjectDescriptorProviders.Add(new FlaggedEnumObjectDescriptorProvider());
    }

    public HandlebarsTemplate<object, object> Compile(string template) => Handlebars.Compile(template);

    public string Parse(string template, object input)
    {
        var t = Compile(template);
        var json = t(input);
        Deserialize(json);
        return json;
    }

    public dynamic? ParseToObject(string template, object input)
    {
        var t = Compile(template);
        var json = t(input);
        return Deserialize(json);
    }

    private static dynamic? Deserialize(string json)
    {
        try
        {
            // replace tabs with spaces, as tabs make the JSON in the
            // console unreadable:
            json = json.Replace("\t", "  ");

            return JsonConvert.DeserializeObject<dynamic>(json);
        }
        catch (Exception ex)
        {
            throw new InvalidJsonException(ex, json);
        }
    }
}