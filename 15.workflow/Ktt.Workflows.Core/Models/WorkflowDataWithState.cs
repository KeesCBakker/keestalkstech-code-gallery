using WorkflowCore.Interface;

namespace Ktt.Workflows.Core.Models;

public class WorkflowDataWithState : IWorkflowDataWithState
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
    // ▶ Journal Logging
    // ───────────────────────────────────────────────
    public List<string> Journal { get; set; } = [];

    public void AddJournalEntry(IStepExecutionContext context, string message, string stepName)
    {
        var timestamp = DateTime.UtcNow.ToString("O"); // ISO 8601
        Journal.Add($"[{timestamp}] [{stepName}] {message}");
    }
}
