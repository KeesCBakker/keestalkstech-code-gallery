using Ktt.Workflows.Core.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

public abstract class SafeStep : StepBody
{
    private string StepName => GetType().Name;

    protected WorkflowDataWithState Data { get; private set; } = default!;

    protected void Journal(IStepExecutionContext context, string message)
    {
        Data?.AddJournalEntry(context, message, StepName);
    }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        if (context.Workflow.Data is not WorkflowDataWithState data)
        {
            throw new InvalidOperationException("Workflow.Data must inherit from WorkflowDataWithState");
        }

        Data = data;

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
            Data.LastException = WorkflowExceptionInfo.From(ex);

            var duration = DateTime.UtcNow - start;
            Journal(context, $"Exception after {duration.TotalMilliseconds:F0} ms: {ex.Message}");

            throw;
        }
    }

    protected abstract ExecutionResult Execute(IStepExecutionContext context);
}
