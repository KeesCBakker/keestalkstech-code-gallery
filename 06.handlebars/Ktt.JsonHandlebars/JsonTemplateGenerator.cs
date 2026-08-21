using HandlebarsDotNet;
using Newtonsoft.Json;

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

    public HandlebarsTemplate<object, object> Compile(string templateSource) => Handlebars.Compile(templateSource);

    public string Parse(string templateSource, object input)
    {
        var t = Compile(templateSource);
        var json = t(input);
        Deserialize(json);
        return json;
    }

    public dynamic? ParseToObject(string templateSource, object input)
    {
        var t = Compile(templateSource);
        var json = t(input);
        return Deserialize(json);
    }

    private static dynamic? Deserialize(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<dynamic>(json);
        }
        catch (Exception ex)
        {
            throw new InvalidJsonException(ex, json);
        }
    }
}
