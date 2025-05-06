using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Steps;
using Ktt.Workflows.Core.Workflows;
using Ktt.Workflows.Implementation.Steps.GitHub;
using Ktt.Workflows.Implementation.Steps.Jenkins;
using Ktt.Workflows.Implementation.Steps.Resources;
using WorkflowCore.Interface;
using WorkflowCore.Models;

using static Ktt.Workflows.Implementation.Steps.Jenkins.TerraformWithJenkinsStep;

namespace Ktt.Workflows.Implementation.Workflows.Provisioning;

public abstract class AddBlazeResourceWorkflow<TData, TStep> : WorkflowBase<TData>
    where TData : IInstanceDefinition, new()
    where TStep : IStepBody
{
    protected abstract string TerraformFileName { get; }

    protected virtual string Repository => "platform-infra";

    protected virtual int PasswordLength => 32;

    public sealed override void Build(IWorkflowBuilder<TData> builder)
    {
        const int total = 7;

        var step1 = builder
            .Status($"1/{total} Validating input...")
            .Then<ValidateWorkflowDataStep>()
                .OnError(WorkflowErrorHandling.Terminate)

            .Status($"2/{total} Creating GitHub branch...")
            .Then<CreateGitHubBranch>()
                .SafeInput(x => x.Definition, data => new()
                {
                    Repository = Repository,
                    Branch = data.BranchName
                })

            .Status($"3/{total} Generating password...")
            .Then<GeneratePasswordStep>()
                .SafeInput(x => x.Length, _ => PasswordLength)

            .Status($"4/{total} Adding Terraform...");

        var step2 = BuildAddTerraformStep(step1);

        step2
            .Status($"5/{total} Planning Terraform...")
            .Then<TerraformWithJenkinsStep>()
                .SafeInput(x => x.TerraformData, data => new()
                {
                    Environment = data.Environment,
                    Branch = data.BranchName,
                    Action = TerraformAction.Plan
                })

            .Status($"6/{total} Applying Terraform...")
            .Then<TerraformWithJenkinsStep>()
                .SafeInput(x => x.TerraformData, data => new()
                {
                    Environment = data.Environment,
                    Branch = data.BranchName,
                    Action = TerraformAction.Apply
                })

            .Finish($"7/{total} Finished")
            .OnError(WorkflowErrorHandling.Terminate);
    }

    protected abstract IStepBuilder<TData, TStep> BuildAddTerraformStep(
        IStepBuilder<TData, StatusStep> builder
    );
}
