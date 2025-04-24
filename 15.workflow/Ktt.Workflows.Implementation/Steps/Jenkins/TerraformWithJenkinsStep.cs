using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps.Jenkins;

public class TerraformWithJenkinsStep : RunJenkinsJobStep
{
    public TerraformJobDefinition DefinitionInput { get; set; } = default!;

    protected override ExecutionResult Execute(IStepExecutionContext context)
    {
        var d = DefinitionInput;

        Definition = new JenkinsJobDefinition(
            JobName: "platform-provisioning-terraform",
            Parameters: new Dictionary<string, string>
            {
                ["environment"] = d.Environment,
                ["branch"] = d.Branch,
                ["action"] = d.Action.ToString().ToLowerInvariant()
            }
        );

        return base.Execute(context);
    }

    public class TerraformJobDefinition
    {
        public string Environment { get; set; } = default!;
        public string Branch { get; set; } = default!;
        public TerraformAction Action { get; set; } = default!;
    }

    public enum TerraformAction
    {
        Plan,
        Apply
    }
}
