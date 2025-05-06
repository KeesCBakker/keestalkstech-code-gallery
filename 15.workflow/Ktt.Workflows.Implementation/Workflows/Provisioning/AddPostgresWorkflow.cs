using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Steps;
using Ktt.Workflows.Implementation.Steps.Resources;
using Ktt.Workflows.Implementation.Workflows.Provisioning;
using WorkflowCore.Interface;

namespace Ktt.Workflows.Implementation.Workflows;

public sealed class AddPostgresWorkflow : AddBlazeResourceWorkflow<AddPostgresWorkflowData, AddPostgresTerraformStep>
{
    protected override string TerraformFileName => "postgres.tf";

    protected override IStepBuilder<AddPostgresWorkflowData, AddPostgresTerraformStep> BuildAddTerraformStep(IStepBuilder<AddPostgresWorkflowData, StatusStep> builder)
    {
        return builder.Then<AddPostgresTerraformStep>()
            .Input(x => x.Instance, data => data)
            .Input(x => x.Password, data => data.GetRequiredFormValue(WorkflowFormKeys.GeneratedPassword))
            .Input(x => x.Definition, data => new()
            {
                Repository = "platform-infra",
                Branch = data.BranchName,
                FilePath = $"{data.Environment}/{TerraformFileName}"
            });
    }
}
