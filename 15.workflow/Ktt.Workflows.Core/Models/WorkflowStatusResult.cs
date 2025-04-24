namespace Ktt.Workflows.Core.Models;

public record WorkflowStatusResult(
    string WorkflowId,
    string StatusText,
    WorkflowExecutionState Status
);

