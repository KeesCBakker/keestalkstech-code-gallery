using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps.Jenkins;

public class TerraformWithJenkinsStep : RunJenkinsJobStep
{
    public TerraformJobDefinition TerraformData { get; set; } = default!;

    protected override Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
    {
        Definition = new()
        {
            JobName = "platform-provisioning-terraform",
            Parameters = new Dictionary<string, string>
            {
                ["environment"] = TerraformData.Environment,
                ["branch"] = TerraformData.Branch,
                ["action"] = TerraformData.Action.ToString().ToLowerInvariant()
            }
        };

        return base.ExecuteAsync(context);
    }

    public class TerraformJobDefinition
    {
        public required string Environment { get; set; }

        public required string Branch { get; set; }

        public required TerraformAction Action { get; set; }

        public required string GitHubPullRequestUrl { get; set; }
    }

    public enum TerraformAction
    {
        Plan,
        Apply
    }
}

