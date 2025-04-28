using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Core.Workflows;
using Ktt.Workflows.Implementation.Steps.GitHub;
using Ktt.Workflows.Implementation.Steps.Jenkins;
using Ktt.Workflows.Implementation.Steps.Resources;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using static Ktt.Workflows.Implementation.Steps.Jenkins.TerraformWithJenkinsStep;
using static Ktt.Workflows.Implementation.Steps.Resources.AddPostgresTerraformStep;

namespace Ktt.Workflows.Implementation.Workflows.Provisioning;

public class AddAtlasPostgresWorkflowData : WorkflowDataWithState, IPostgresInstanceDefinition
{
    public string Environment { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Team { get; set; } = default!;

    public int StorageInGb { get; set; }

    public string InstanceType { get; set; } = default!;

    public string BranchName => $"add-postgres-{Name}-to-{Environment}";

    public string FilePath => $"{Team}/{Name}/{Environment}/postgres.tf";
}

[AutoRegisterWorkflow]
public class AddAtlasPostgresWorkflow : IWorkflow<AddAtlasPostgresWorkflowData>
{
    public string Id => "AddAtlasPostgres";
    public int Version => 1;

    public void Build(IWorkflowBuilder<AddAtlasPostgresWorkflowData> builder)
    {
        const int total = 9;
        const string atlasRepo = "atlas-application-provisioning";

        builder
            .Status($"1/{total} Creating GitHub branch...")
            .Then<CreateGitHubBranch>()
                .Input(x => x.Definition, data => new()
                {
                    Repository = atlasRepo,
                    Branch = data.BranchName
                })

            .Status($"2/{total} Ensuring Atlas directory...")
            .Then<EnsureAtlasApplicationDirectoryStep>()
                .Input(x => x.ApplicationName, data => data.Name)
                .Input(x => x.Environment, data => data.Environment)
                .Input(x => x.Repository, _ => atlasRepo)

            .Status($"3/{total} Generating database password...")
            .Then<GeneratePasswordStep>()
                .Name("GenerateDbPassword")
                .Input(x => x.Length, _ => 32)

            .Status($"4/{total} Adding Terraform for Postgres...")
            .Then<AddPostgresTerraformStep>()
                .Input(x => x.Instance, data => data)
                .Input(x => x.Password, data => data.GetRequiredFormValue(WorkflowFormKeys.GeneratedPassword))
                .Input(x => x.Definition, data => new()
                {
                    Repository = atlasRepo,
                    Branch = data.BranchName,
                    FilePath = data.FilePath
                })

            .Status($"5/{total} Creating pull request...")
            .Then<CreateGitHubPr>()
                .Input(x => x.Definition, data => new()
                {
                    Repository = atlasRepo,
                    Branch = data.BranchName,
                    Description = $"Provision Postgres instance for {data.Team}/{data.Name} in {data.Environment}"
                })

            .Status($"6/{total} Planning Terraform...")
            .Then<TerraformWithJenkinsStep>()
                .Input(x => x.TerraformData, data => new()
                {
                    Environment = data.Environment,
                    Branch = data.BranchName,
                    Action = TerraformAction.Plan
                })

            .Status($"7/{total} Applying Terraform...")
            .Then<TerraformWithJenkinsStep>()
                .Input(x => x.TerraformData, data => new()
                {
                    Environment = data.Environment,
                    Branch = data.BranchName,
                    Action = TerraformAction.Apply
                })

            .Status($"8/{total} Resolving URL...")
            .Then<ProcessTerraformOutputStep>()
                .Input(x => x.Process, _ => (c, d, _) => d.SetFormValue("length", c.Length.ToString()))

            .Finish($"9/{total} Finished adding Postgres instance.");
    }
}
