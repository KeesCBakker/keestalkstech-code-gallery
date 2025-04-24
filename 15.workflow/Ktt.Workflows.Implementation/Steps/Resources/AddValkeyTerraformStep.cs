using Ktt.Workflows.Implementation.Steps.GitHub;

namespace Ktt.Workflows.Implementation.Steps.Resources;
public class AddValkeyTerraformStep : EditGitHubFile
{
    public IValkeyInstanceDefinition Instance { get; set; } = default!;

    protected override string Edit(string currentContent)
    {
        var i = Instance;

        var addition = $@"
resource ""valkey_instance"" ""{i.Name}"" {{
  name          = ""{i.Name}""
  instance_type = ""{i.InstanceType}""
}}";

        return currentContent.TrimEnd() + "\n\n" + addition + "\n";
    }

    public interface IValkeyInstanceDefinition
    {
        string Environment { get; }
        string Name { get; }
        string InstanceType { get; }
    }
}
