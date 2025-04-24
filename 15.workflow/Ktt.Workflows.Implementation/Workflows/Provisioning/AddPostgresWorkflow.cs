using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Implementation.Steps.GitHub;
using Ktt.Workflows.Implementation.Steps.Jenkins;
using Ktt.Workflows.Implementation.Steps.Resources;
using WorkflowCore.Interface;
using static Ktt.Workflows.Implementation.Steps.Jenkins.TerraformWithJenkinsStep;
using static Ktt.Workflows.Implementation.Steps.Resources.AddPostgresTerraformStep;

namespace Ktt.Workflows.Implementation.Workflows;

public class AddPostgresWorkflowData : WorkflowDataWithState, IPostgresInstanceDefinition
{
    public string Environment { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Team { get; set; } = default!;

    public int StorageInGb { get; set; } = default!;

    public string InstanceType { get; set; } = default!;

    public string BranchName => $"add-postgres-{Name}-to-{Environment}";
}

[AutoRegisterWorkflow]
public class AddPostgresWorkflow : IWorkflow<AddPostgresWorkflowData>
{
    public string Id => "AddPostgres";
    public int Version => 1;

    public void Build(IWorkflowBuilder<AddPostgresWorkflowData> builder)
    {
        var repository = "platform-infra";

        builder
            .StartWith<CreateGitHubBranch>()
                .Input(x => x.Definition, data => new CreateGitHubBranch.GitHubBranchDefinition
                {
                    Repository = repository,
                    Branch = data.BranchName
                })

            .Then<AddPostgresTerraformStep>()
                .Input(x => x.Instance, data => data)
                .Input(x => x.Definition, data => new EditGitHubFile.GitHubFileDefinition
                {
                    Repository = repository,
                    Branch = data.BranchName,
                    FilePath = $"{data.Environment}/postgres.tf"
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
