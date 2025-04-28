using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Core.Steps;
using Ktt.Workflows.Core.Workflows;
using Ktt.Workflows.Implementation.Steps.GitHub;
using Ktt.Workflows.Implementation.Steps.Jenkins;
using Ktt.Workflows.Implementation.Steps.Resources;
using WorkflowCore.Interface;
using static Ktt.Workflows.Implementation.Steps.Jenkins.TerraformWithJenkinsStep;

namespace Ktt.Workflows.Implementation.Workflows.Provisioning;

public interface IInstanceDefinition : IWorkflowDataWithState
{
    string Environment { get; }

    string Name { get; }

    string BranchName { get; }
}

public abstract class AddBlazeResourceWorkflow<TData, TStep> : IWorkflow<TData>
    where TData : IInstanceDefinition, new()
    where TStep : IStepBody
{
    public abstract string Id { get; }

    public int Version { get; protected set; } = 1;

    protected abstract string TerraformFileName { get; }

    protected virtual string Repository => "platform-infra";

    protected virtual int PasswordLength => 32;

    public void Build(IWorkflowBuilder<TData> builder)
    {
        const int total = 6;

        var step1 = builder
            .Status($"1/{total} Creating GitHub branch...")
            .Then<CreateGitHubBranch>()
                .Input(x => x.Definition, data => new()
                {
                    Repository = Repository,
                    Branch = data.BranchName
                })

            .Status($"2/{total} Generating password...")
            .Then<GeneratePasswordStep>()
                .Input(x => x.Length, _ => PasswordLength)

            .Status($"3/{total} Adding Terraform...");

        var step2 = BuildAddTerraformStep(step1);

        step2
            .Status($"4/{total} Planning Terraform...")
            .Then<TerraformWithJenkinsStep>()
                .Input(x => x.TerraformData, data => new()
                {
                    Environment = data.GetRequiredFormValue("Kaas") + data.Environment,
                    Branch = data.BranchName,
                    Action = TerraformAction.Plan
                })

            .Status($"5/{total} Applying Terraform...")
            .Then<TerraformWithJenkinsStep>()
                .Input(x => x.TerraformData, data => new()
                {
                    Environment = data.Environment,
                    Branch = data.BranchName,
                    Action = TerraformAction.Apply
                })

            .Finish($"6/{total} Finished")
            .OnError(WorkflowCore.Models.WorkflowErrorHandling.Terminate);
    }

    protected abstract IStepBuilder<TData, TStep> BuildAddTerraformStep(
        IStepBuilder<TData, StatusStep> builder
    );
}
