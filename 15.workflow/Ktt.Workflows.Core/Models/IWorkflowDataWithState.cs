using WorkflowCore.Interface;

namespace Ktt.Workflows.Core.Models;

public interface IWorkflowDataWithState
{
    T GetStepState<T>(IStepExecutionContext context) where T : new();

    void SetStepState<T>(IStepExecutionContext context, T value);

    void AddJournalEntry(IStepExecutionContext context, string message, string stepName);

    string Status { get; set; }

    WorkflowExceptionInfo? LastException { get; set; }

    List<string> Journal { get; }
}
