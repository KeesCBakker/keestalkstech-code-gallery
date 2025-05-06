using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Core.Steps;

public class DelayStep : SafeStep
{
    public TimeSpan Delay { get; set; }

    protected override async Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
    {
        if (Delay <= TimeSpan.Zero)
        {
            Journal(context, "No delay specified, proceeding immediately.");
            return await Next();
        }

        if (context.PersistenceData == null)
        {
            Journal(context, $"Sleeping for {Delay.TotalSeconds:F0} seconds.");
            return await Sleep(Delay);
        }

        return ExecutionResult.Next();
    }
}
