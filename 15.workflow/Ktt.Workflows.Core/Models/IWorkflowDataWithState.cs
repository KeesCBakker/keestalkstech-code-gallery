using WorkflowCore.Interface;

namespace Ktt.Workflows.Core.Models;

public interface IWorkflowDataWithState
{
    string StatusTitle { get; set; }

    string StatusDescription { get; set; }

    Dictionary<string, string> Form { get; set; }

    WorkflowExecutionState State { get; set; }

    WorkflowExceptionInfo? LastException { get; set; }

    Dictionary<string, object> StepState { get; }

    List<string> Journal { get; set; }

    T GetStepState<T>(IStepExecutionContext context) where T : new();

    void SetStepState<T>(IStepExecutionContext context, T value);

    T? GetLastStepState<T>(string stepName) where T : class;

    void AddJournalEntry(IStepExecutionContext context, string message, string stepName);
}
