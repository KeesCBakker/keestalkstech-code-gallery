using WorkflowCore.Interface;

namespace Ktt.Workflows.Core.Models;

public interface IWorkflowDataWithState
{
    // ───────────────────────────────────────────────
    // ▶ Workflow Status
    // ───────────────────────────────────────────────
    string StatusTitle { get; set; }
    string StatusDescription { get; set; }
    WorkflowExecutionState State { get; set; }
    WorkflowExceptionInfo? LastException { get; set; }

    // ───────────────────────────────────────────────
    // ▶ Workflow Form
    // ───────────────────────────────────────────────
    Dictionary<string, string> Form { get; set; }
    void SetFormValue(string key, string value);
    string GetRequiredFormValue(string key);

    // ───────────────────────────────────────────────
    // ▶ Per-step State
    // ───────────────────────────────────────────────
    Dictionary<string, object> StepState { get; }
    T GetStepState<T>(IStepExecutionContext context) where T : new();
    void SetStepState<T>(IStepExecutionContext context, T value);
    T? GetLastStepState<T>(string stepName) where T : class;

    // ───────────────────────────────────────────────
    // ▶ Journal Logging
    // ───────────────────────────────────────────────
    List<string> Journal { get; set; }
    void AddJournalEntry(IStepExecutionContext context, string message, string stepName);
}
