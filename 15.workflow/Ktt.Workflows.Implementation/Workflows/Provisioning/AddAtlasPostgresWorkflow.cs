using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Workflows;
using Ktt.Workflows.Implementation.Steps.GitHub;
using Ktt.Workflows.Implementation.Steps.Jenkins;
using Ktt.Workflows.Implementation.Steps.Resources;
using WorkflowCore.Interface;
using static Ktt.Workflows.Implementation.Steps.Jenkins.TerraformWithJenkinsStep;

namespace Ktt.Workflows.Implementation.Workflows.Provisioning;

public sealed class AddAtlasPostgresWorkflow : WorkflowBase<AddAtlasPostgresWorkflowData>
{
    public sealed override void Build(IWorkflowBuilder<AddAtlasPostgresWorkflowData> builder)
    {
        const int total = 9;
        const string atlasRepo = "atlas-application-provisioning";

        builder
            .Status($"1/{total} Creating GitHub branch...")
            .Then<CreateGitHubBranch>()
                .SafeInput(x => x.Definition, data => new()
                {
                    Repository = atlasRepo,
                    Branch = data.BranchName
                })

            .Status($"2/{total} Ensuring Atlas directory...")
            .Then<EnsureAtlasApplicationDirectoryStep>()
                .SafeInput(x => x.ApplicationName, data => data.Name)
                .SafeInput(x => x.Environment, data => data.Environment)
                .SafeInput(x => x.Repository, _ => atlasRepo)

            .Status($"3/{total} Generating database password...")
            .Then<GeneratePasswordStep>()
                .Name("GenerateDbPassword")
                .SafeInput(x => x.Length, _ => 32)

            .Status($"4/{total} Adding Terraform for Postgres...")
            .Then<AddPostgresTerraformStep>()
                .SafeInput(x => x.Instance, data => data)
                .SafeInput(x => x.Password, data => data.GetRequiredFormValue(WorkflowFormKeys.GeneratedPassword))
                .SafeInput(x => x.Definition, data => new()
                {
                    Repository = atlasRepo,
                    Branch = data.BranchName,
                    FilePath = data.FilePath
                })

            .Status($"5/{total} Creating pull request...")
            .Then<CreateGitHubPr>()
                .SafeInput(x => x.Definition, data => new()
                {
                    Repository = atlasRepo,
                    Branch = data.BranchName,
                    Description = $"Provision Postgres instance for {data.Team}/{data.Name} in {data.Environment}"
                })

            .Status($"6/{total} Planning Terraform...")
            .Then<TerraformWithJenkinsStep>()
                .SafeInput(x => x.TerraformData, data => new()
                {
                    Environment = data.Environment,
                    Branch = data.BranchName,
                    Action = TerraformAction.Plan
                })

            .Status($"7/{total} Applying Terraform...")
            .Then<TerraformWithJenkinsStep>()
                .SafeInput(x => x.TerraformData, data => new()
                {
                    Environment = data.Environment,
                    Branch = data.BranchName,
                    Action = TerraformAction.Apply
                })

            .Status($"8/{total} Resolving URL...")
            .Then<ProcessTerraformOutputStep>()
                .SafeInput(x => x.Process, _ => (c, d, _) => d.SetFormValue("length", c.Length.ToString()))

            .Finish($"9/{total} Finished adding Postgres instance.");
    }
}
