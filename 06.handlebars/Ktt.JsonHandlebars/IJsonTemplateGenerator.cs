using HandlebarsDotNet;

namespace Ktt.JsonHandlebars;

public interface IJsonTemplateGenerator
{
    IHandlebars Handlebars { get; }

    HandlebarsTemplate<object, object> Compile(string templateSource);

    string Parse(string templateSource, object input);

    dynamic? ParseToObject(string templateSource, object input);
}
