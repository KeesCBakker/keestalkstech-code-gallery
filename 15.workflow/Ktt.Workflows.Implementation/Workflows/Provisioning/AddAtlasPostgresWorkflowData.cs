using Ktt.Workflows.Core.Models;
using System.ComponentModel.DataAnnotations;
using static Ktt.Workflows.Implementation.Steps.Resources.AddPostgresTerraformStep;

namespace Ktt.Workflows.Implementation.Workflows.Provisioning;

public class AddAtlasPostgresWorkflowData : WorkflowDataWithState, IPostgresInstanceDefinition
{
    public string Environment { get; set; } = default!;

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string Team { get; set; } = default!;

    [Range(1, 100)]
    public int StorageInGb { get; set; }

    [Required]
    public string InstanceType { get; set; } = default!;

    public string BranchName => $"add-postgres-{Name}-to-{Environment}";

    public string FilePath => $"{Team}/{Name}/{Environment}/postgres.tf";
}
