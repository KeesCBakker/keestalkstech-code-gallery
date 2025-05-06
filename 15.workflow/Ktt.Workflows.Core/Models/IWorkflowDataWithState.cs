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
    // ▶ Journal Logging
    // ───────────────────────────────────────────────
    List<string> Journal { get; set; }
    void AddJournalEntry(IStepExecutionContext context, string message, string stepName);
}
