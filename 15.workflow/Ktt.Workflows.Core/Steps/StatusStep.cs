using Ktt.Workflows.Core.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Core.Steps;

public class StatusStep : SafeStep
{
    public string StatusTitle { get; set; } = string.Empty;

    public string StatusDescription { get; set; } = string.Empty;

    public WorkflowExecutionState? State { get; set; }

    protected override Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
    {
        if (context.Workflow.Data is not IWorkflowDataWithState data)
        {
            throw new InvalidOperationException("Expected LeetMathWorkflowData");
        }

        data.StatusTitle = StatusTitle;
        data.StatusDescription = StatusDescription;

        if (State != null)
        {
            data.State = State.Value;
        }

        return Next();
    }
}
