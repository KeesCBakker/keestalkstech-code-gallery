using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.App.Workflows.Steps;

public class MultiplyStepInput
{
    public int Multiplier { get; set; }
}

public class MultiplyStep : StepBody
{
    public MultiplyStepInput Input { get; set; } = default!;
    public int CurrentNumber { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        CurrentNumber *= Input.Multiplier;
        return ExecutionResult.Next();
    }
}
