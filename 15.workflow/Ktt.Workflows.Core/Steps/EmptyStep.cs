using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Core.Steps;

public class EmptyStep : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        return ExecutionResult.Next();
    }
}
