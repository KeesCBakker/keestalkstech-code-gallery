using System.Diagnostics;

namespace Ktt.Workflows.Core.Models;

[DebuggerDisplay("{WorkflowDefinitionId,nq} workflow: {StatusTitle}")]
public class WorkflowStatusResult
{
    public required string WorkflowId { get; init; }

    public required string StatusTitle { get; init; }

    public required string StatusDescription { get; init; }

    public string LastExceptionMessage { get; init; } = string.Empty;

    public WorkflowExecutionState State { get; init; }

    public required Dictionary<string, string> Form { get; init; }

    public required List<string> Journal { get; init; }

    public bool IsRunning => State == WorkflowExecutionState.Running;

    public bool IsDone => !IsRunning;

    public required string WorkflowDefinitionId { get; init; }
}
