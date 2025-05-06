using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Implementation.Workflows.Provisioning;
using System.ComponentModel.DataAnnotations;
using static Ktt.Workflows.Implementation.Steps.Resources.AddValkeyTerraformStep;

namespace Ktt.Workflows.Implementation.Workflows;

public class AddValkeyWorkflowData : WorkflowDataWithState, IValkeyInstanceDefinition, IInstanceDefinition
{
    [Required]
    public string Environment { get; set; } = default!;

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string InstanceType { get; set; } = default!;

    public string BranchName => $"add-valkey-{Name}-to-{Environment}";
}
