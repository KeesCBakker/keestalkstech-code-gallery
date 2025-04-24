using Ktt.Workflows.Core.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Core.Steps;

public class StatusStep : SafeStep
{
    public string Status { get; set; } = string.Empty;

    protected override ExecutionResult Execute(IStepExecutionContext context)
    {
        if (context.Workflow.Data is not IWorkflowDataWithState data)
        {
            throw new InvalidOperationException("Expected LeetMathWorkflowData");
        }

        data.Status = Status;

        return ExecutionResult.Next();
    }
}
