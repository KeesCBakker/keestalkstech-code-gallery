using System.Reflection;

namespace Ktt.JsonHandlebars;

public class ManifestResourceNotFoundException(string name, Assembly assembly)
    : Exception($"Manifest resource with name '{name}' in assembly '{assembly.FullName}' not found.")
{
}
