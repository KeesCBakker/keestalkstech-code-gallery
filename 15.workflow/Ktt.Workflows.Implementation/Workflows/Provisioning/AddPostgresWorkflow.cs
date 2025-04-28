using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Core.Steps;
using Ktt.Workflows.Implementation.Steps.Resources;
using Ktt.Workflows.Implementation.Workflows.Provisioning;
using WorkflowCore.Interface;
using static Ktt.Workflows.Implementation.Steps.Resources.AddPostgresTerraformStep;

namespace Ktt.Workflows.Implementation.Workflows;

public class AddPostgresWorkflowData : WorkflowDataWithState, IPostgresInstanceDefinition, IInstanceDefinition
{
    public string Environment { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Team { get; set; } = default!;

    public int StorageInGb { get; set; } = default!;

    public string InstanceType { get; set; } = default!;

    public string BranchName => $"add-postgres-{Name}-to-{Environment}";
}

[AutoRegisterWorkflow]
public class AddPostgresWorkflow : AddBlazeResourceWorkflow<AddPostgresWorkflowData, AddPostgresTerraformStep>
{
    public override string Id => "AddPostgres";

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
