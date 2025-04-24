using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.App.Workflows.Steps;

public class AddStepInput
{
    public int Value { get; set; }
}

public class AddStep : StepBody
{
    public AddStepInput Input { get; set; } = default!;
    public int CurrentNumber { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        CurrentNumber += Input.Value;
        return ExecutionResult.Next();
    }
}
