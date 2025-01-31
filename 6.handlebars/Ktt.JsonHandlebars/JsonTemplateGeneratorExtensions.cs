using System.Reflection;

namespace Ktt.JsonHandlebars;

public static class JsonTemplateGeneratorExtensions
{
    public static string ParseWithManifestResource(this IJsonTemplateGenerator generator, string name, object input)
    {
        return generator.ParseWithManifestResource(Assembly.GetCallingAssembly(), name, input);
    }

    public static string ParseWithManifestResource(this IJsonTemplateGenerator generator, Assembly assembly, string name, object input)
    {
        var template = GetManifestTemplate(assembly, name, input);
        return generator.Parse(template, input);
    }

    public static dynamic? ParseWithManifestResourceToObject(this IJsonTemplateGenerator generator, string name, object input)
    {
        return generator.ParseWithManifestResourceToObject(Assembly.GetCallingAssembly(), name, input);
    }

    public static dynamic? ParseWithManifestResourceToObject(this IJsonTemplateGenerator generator, Assembly assembly, string name, object input)
    {
        var template = GetManifestTemplate(assembly, name, input);
        return generator.ParseToObject(template, input);
    }

    private static string GetManifestTemplate(Assembly assembly, string name, object input)
    {
        using var stream = assembly.GetManifestResourceStream(name) ?? throw new Exception($"Manifest resouce with name '{name}' in assembly '{assembly.FullName}' not found.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
