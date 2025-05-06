using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Implementation.Workflows.Provisioning;
using System.ComponentModel.DataAnnotations;
using static Ktt.Workflows.Implementation.Steps.Resources.AddPostgresTerraformStep;

namespace Ktt.Workflows.Implementation.Workflows;

public class AddPostgresWorkflowData : WorkflowDataWithState, IPostgresInstanceDefinition, IInstanceDefinition
{
    [Required]
    public string Environment { get; set; } = default!;

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string Team { get; set; } = default!;

    [Range(minimum: 1, maximum: 100)]
    public int StorageInGb { get; set; } = default!;

    [Required]
    public string InstanceType { get; set; } = default!;

    public string BranchName => $"add-postgres-{Name}-to-{Environment}";
}
