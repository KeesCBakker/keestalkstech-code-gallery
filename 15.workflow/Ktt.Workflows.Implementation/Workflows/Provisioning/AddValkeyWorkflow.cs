using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Core.Steps;
using Ktt.Workflows.Implementation.Steps.Resources;
using Ktt.Workflows.Implementation.Workflows.Provisioning;
using WorkflowCore.Interface;
using static Ktt.Workflows.Implementation.Steps.Resources.AddValkeyTerraformStep;

namespace Ktt.Workflows.Implementation.Workflows;

public class AddValkeyWorkflowData : WorkflowDataWithState, IValkeyInstanceDefinition, IInstanceDefinition
{
    public string Environment { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string InstanceType { get; set; } = default!;

    public string BranchName => $"add-valkey-{Name}-to-{Environment}";
}

[AutoRegisterWorkflow]
public class AddValkeyWorkflow : AddBlazeResourceWorkflow<AddValkeyWorkflowData, AddValkeyTerraformStep>
{
    public override string Id => "AddValkey";
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
