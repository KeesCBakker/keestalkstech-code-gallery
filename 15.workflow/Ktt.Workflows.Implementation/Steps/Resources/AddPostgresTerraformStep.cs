using Ktt.Workflows.Implementation.Steps.GitHub;

namespace Ktt.Workflows.Implementation.Steps.Resources;

public class AddPostgresTerraformStep : EditGitHubFile
{
    public IPostgresInstanceDefinition Instance { get; set; } = default!;
    public string Password { get; set; } = default!;

    protected override string Edit(string currentContent)
    {
        var i = Instance;

        var addition = $@"
resource ""postgres_instance"" ""{i.Name}"" {{
  name           = ""{i.Name}""
  team           = ""{i.Team}""
  storage_gb     = {i.StorageInGb}
  instance_type  = ""{i.InstanceType}""
  password       = ""{Password}""
}}";

        return currentContent.TrimEnd() + "\n\n" + addition + "\n";
    }

    public interface IPostgresInstanceDefinition
    {
        string Environment { get; }
        string Name { get; }
        string Team { get; }
        int StorageInGb { get; }
        string InstanceType { get; }
    }
}
