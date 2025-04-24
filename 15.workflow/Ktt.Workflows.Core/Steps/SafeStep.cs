using Ktt.Workflows.Core.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Core.Steps;

public abstract class SafeStep : StepBody
{
    private string StepName => GetType().Name;

    protected void Journal(IStepExecutionContext context, string message)
    {
        if (context.Workflow.Data is IWorkflowDataWithState data)
        {
            data.AddJournalEntry(context, message, StepName);
        }
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var start = DateTime.UtcNow;

        Journal(context, "Entering step");

        try
        {
            var result = Execute(context);

            var duration = DateTime.UtcNow - start;
            Journal(context, $"Exiting step (Duration: {duration.TotalMilliseconds:F0} ms)");

            return result;
        }
        catch (Exception ex)
        {
            if (context.Workflow.Data is IWorkflowDataWithState data)
            {
                data.LastException = WorkflowExceptionInfo.From(ex);
            }

            var duration = DateTime.UtcNow - start;
            Journal(context, $"Exception after {duration.TotalMilliseconds:F0} ms: {ex.Message}");

            throw;
        }
    }

    protected abstract ExecutionResult Execute(IStepExecutionContext context);
}
