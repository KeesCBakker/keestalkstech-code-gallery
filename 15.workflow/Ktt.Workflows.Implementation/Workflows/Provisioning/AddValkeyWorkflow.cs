using WorkflowCore.Interface;
using Ktt.Workflows.Implementation.Steps.GitHub;
using Ktt.Workflows.Implementation.Steps.Jenkins;
using Ktt.Workflows.Implementation.Steps.Resources;
using static Ktt.Workflows.Implementation.Steps.Jenkins.TerraformWithJenkinsStep;
using Ktt.Workflows.Core;

namespace Ktt.Workflows.Implementation.Workflows;

[AutoRegisterWorkflow]
public class AddValkeyWorkflow : IWorkflow<AddValkeyWorkflowData>
{
    public string Id => "AddValkey";
    public int Version => 1;

    public void Build(IWorkflowBuilder<AddValkeyWorkflowData> builder)
    {
        const string repository = "platform-infra";

        builder
            .StartWith<CreateGitHubBranch>()
                .Input(x => x.Definition, data => new CreateGitHubBranch.GitHubBranchDefinition
                {
                    Repository = repository,
                    Branch = data.BranchName
                })

            .Then<AddValkeyTerraformStep>()
                .Input(x => x.Instance, data => data)
                .Input(x => x.Definition, data => new EditGitHubFile.GitHubFileDefinition
                {
                    Repository = repository,
                    Branch = data.BranchName,
                    FilePath = $"{data.Environment}/valkey.tf"
                })

            .Then<TerraformWithJenkinsStep>()
                .Input(x => x.DefinitionInput, data => new TerraformJobDefinition
                {
                    Environment = data.Environment,
                    Branch = data.BranchName,
                    Action = TerraformAction.Plan
                })

            .Then<TerraformWithJenkinsStep>()
                .Input(x => x.DefinitionInput, data => new TerraformJobDefinition
                {
                    Environment = data.Environment,
                    Branch = data.BranchName,
                    Action = TerraformAction.Apply
                });
    }
}
