using Ktt.Workflows.Core.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

public abstract class SafeStep : StepBodyAsync
{
    private string StepName => GetType().Name;

    protected WorkflowDataWithState Data { get; private set; } = default!;

    protected void Journal(IStepExecutionContext context, string message)
    {
        Data?.AddJournalEntry(context, message, StepName);
    }

    public async override sealed Task<ExecutionResult> RunAsync(IStepExecutionContext context)
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
            var result = await ExecuteAsync(context);

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

    protected abstract Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context);

    protected static Task<ExecutionResult> Next()
    {
        return Task.FromResult(ExecutionResult.Next());
    }

    protected static Task<ExecutionResult> Sleep(TimeSpan ts)
    {
        return Task.FromResult(ExecutionResult.Sleep(ts, new object()));
    }
}
