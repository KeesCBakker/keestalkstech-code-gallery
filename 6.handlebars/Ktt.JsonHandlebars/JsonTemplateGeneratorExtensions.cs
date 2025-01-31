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
        var template = GetManifestTemplate(assembly, name);
        return generator.Parse(template, input);
    }

    public static dynamic? ParseWithManifestResourceToObject(this IJsonTemplateGenerator generator, string name, object input)
    {
        return generator.ParseWithManifestResourceToObject(Assembly.GetCallingAssembly(), name, input);
    }

    public static dynamic? ParseWithManifestResourceToObject(this IJsonTemplateGenerator generator, Assembly assembly, string name, object input)
    {
        var template = GetManifestTemplate(assembly, name);
        return generator.ParseToObject(template, input);
    }

    private static string GetManifestTemplate(Assembly assembly, string name)
    {
        using var stream = assembly.GetManifestResourceStream(name) ?? throw new ManifestResourceNotFoundException(name, assembly);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}

class ManifestResourceNotFoundException(string name, Assembly assembly) : Exception($"Manifest resouce with name '{name}' in assembly '{assembly.FullName}' not found.")
{
}
