using WorkflowCore.Interface;

namespace Ktt.Workflows.Core.Models;

public abstract class WorkflowDataWithState : IWorkflowDataWithState
{
    public string Status { get; set; } = string.Empty;

    public WorkflowExceptionInfo? LastException { get; set; }

    public Dictionary<string, object> StepState { get; init; } = new();

    private static string BuildKey(IStepExecutionContext context)
        => $"exec:{context.ExecutionPointer.Id}:{context.Step.BodyType.Name}";

    public T GetStepState<T>(IStepExecutionContext context) where T : new()
    {
        var key = BuildKey(context);
        if (StepState.TryGetValue(key, out var obj) && obj is T value)
        {
            return value;
        }

        var fresh = new T();
        StepState[key] = fresh;
        return fresh;
    }

    public void SetStepState<T>(IStepExecutionContext context, T value)
    {
        StepState[BuildKey(context)] = value!;
    }

    public List<string> Journal { get; set; } = [];

    public void AddJournalEntry(IStepExecutionContext context, string message, string stepName)
    {
        var timestamp = DateTime.UtcNow.ToString("O"); // ISO 8601
        Journal.Add($"[{timestamp}] [{stepName}] {message}");
    }
}
