using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Steps;
using Ktt.Workflows.Implementation.Steps.Resources;
using Ktt.Workflows.Implementation.Workflows.Provisioning;
using WorkflowCore.Interface;

namespace Ktt.Workflows.Implementation.Workflows;

public class AddValkeyWorkflow : AddBlazeResourceWorkflow<AddValkeyWorkflowData, AddValkeyTerraformStep>
{
    protected override string TerraformFileName => "valkey.tf";

    protected override IStepBuilder<AddValkeyWorkflowData, AddValkeyTerraformStep> BuildAddTerraformStep(
        IStepBuilder<AddValkeyWorkflowData, StatusStep> builder
    )
    {
        return builder.Then<AddValkeyTerraformStep>()
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
