using HandlebarsDotNet;
using Newtonsoft.Json;

namespace Ktt.JsonHandlebars;

public static class JsonHandlebarsDotNet
{
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
