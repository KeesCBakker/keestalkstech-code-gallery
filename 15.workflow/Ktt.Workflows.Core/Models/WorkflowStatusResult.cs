namespace Ktt.Workflows.Core.Models;

public class WorkflowStatusResult
{
    public required string WorkflowId { get; init; }

    public required string StatusTitle { get; init; }

    public required string StatusDescription { get; init; }

    public WorkflowExecutionState State { get; init; }

    public required Dictionary<string, string> Form { get; init; }

    public required List<string> Journal { get; init; }
}
