using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Core.Workflows;
using Ktt.Workflows.Implementation.Steps.GitHub;
using Ktt.Workflows.Implementation.Steps.Jenkins;
using Ktt.Workflows.Implementation.Steps.Resources;
using WorkflowCore.Interface;
using static Ktt.Workflows.Implementation.Steps.Jenkins.TerraformWithJenkinsStep;
using static Ktt.Workflows.Implementation.Steps.Resources.AddValkeyTerraformStep;

namespace Ktt.Workflows.Implementation.Workflows;

public class AddValkeyWorkflowData : WorkflowDataWithState, IValkeyInstanceDefinition
{
    public string Environment { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string InstanceType { get; set; } = default!;

    public string BranchName => $"add-valkey-{Name}-to-{Environment}";
}

[AutoRegisterWorkflow]
public class AddValkeyWorkflow : IWorkflow<AddValkeyWorkflowData>
{
    public string Id => "AddValkey";
    public int Version => 1;

    public void Build(IWorkflowBuilder<AddValkeyWorkflowData> builder)
    {
        const string repository = "platform-infra";
        const int total = 6;

        builder
            .Status($"1/{total} Creating GitHub branch...")
            .Then<CreateGitHubBranch>()
                .Input(x => x.Definition, data => new()
                {
                    Repository = repository,
                    Branch = data.BranchName
                })

            .Status($"2/{total} Generating password...")
            .Then<GeneratePasswordStep>()
                .Input(x => x.Length, _ => 32)

            .Status($"3/{total} Adding Terraform for Valkey...")
            .Then<AddValkeyTerraformStep>()
                .Input(x => x.Instance, data => data)
                .Input(x => x.Password, data => data.GetRequiredFormValue(WorkflowFormKeys.GeneratedPassword))
                .Input(x => x.Definition, data => new()
                {
                    Repository = repository,
                    Branch = data.BranchName,
                    FilePath = $"{data.Environment}/valkey.tf"
                })

            .Status($"4/{total} Planning Terraform...")
            .Then<TerraformWithJenkinsStep>()
                .Input(x => x.TerraformData, data => new()
                {
                    Environment = data.Environment,
                    Branch = data.BranchName,
                    Action = TerraformAction.Plan,
                    GitHubPullRequestUrl = data.GetRequiredFormValue(WorkflowFormKeys.GitHubPullRequestUrl)
                })

            .Status($"5/{total} Applying Terraform...")
            .Then<TerraformWithJenkinsStep>()
                .Input(x => x.TerraformData, data => new()
                {
                    Environment = data.Environment,
                    Branch = data.BranchName,
                    Action = TerraformAction.Apply,
                    GitHubPullRequestUrl = data.GetRequiredFormValue(WorkflowFormKeys.GitHubPullRequestUrl)
                })

            .Finish($"6/{total} Finished");
    }
}
