namespace Ktt.Workflows.Core.Models;

public record WorkflowExceptionInfo
{
    public string Message { get; init; } = default!;

    public string? Type { get; init; }

    public string? StackTrace { get; init; }

    public static WorkflowExceptionInfo From(Exception ex) => new()
    {
        Message = ex.Message,
        Type = ex.GetType().FullName,
        StackTrace = ex.StackTrace
    };
}
