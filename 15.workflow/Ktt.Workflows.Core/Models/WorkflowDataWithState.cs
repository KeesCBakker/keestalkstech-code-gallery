using WorkflowCore.Interface;

namespace Ktt.Workflows.Core.Models;

public abstract class WorkflowDataWithState : IWorkflowDataWithState
{
    // ───────────────────────────────────────────────
    // ▶ Workflow Status
    // ───────────────────────────────────────────────
    public string StatusTitle { get; set; } = string.Empty;

    public string StatusDescription { get; set; } = string.Empty;

    public WorkflowExecutionState State { get; set; } = WorkflowExecutionState.Running;

    public WorkflowExceptionInfo? LastException { get; set; }

    // ───────────────────────────────────────────────
    // ▶ Workflow Form
    // ───────────────────────────────────────────────
    public Dictionary<string, string> Form { get; set; } = [];

    public void SetFormValue(string key, string value)
    {
        Form ??= [];
        Form[key] = value;
    }

    public string GetRequiredFormValue(string key)
    {
        if (!Form.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Form value '{key}' is required but missing.");
        }

        return value;
    }

    // ───────────────────────────────────────────────
    // ▶ Per-step State
    // ───────────────────────────────────────────────
    public Dictionary<string, object> StepState { get; init; } = [];

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

    public T? GetLastStepState<T>(string stepName) where T : class
    {
        return StepState
            .Where(kvp => kvp.Key.EndsWith($":{stepName}"))
            .Select(kvp => kvp.Value)
            .OfType<T>()
            .LastOrDefault();
    }

    // ───────────────────────────────────────────────
    // ▶ Journal Logging
    // ───────────────────────────────────────────────
    public List<string> Journal { get; set; } = [];

    public void AddJournalEntry(IStepExecutionContext context, string message, string stepName)
    {
        var timestamp = DateTime.UtcNow.ToString("O"); // ISO 8601
        Journal.Add($"[{timestamp}] [{stepName}] {message}");
    }
}
