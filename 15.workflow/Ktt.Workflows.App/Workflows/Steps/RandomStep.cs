using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.App.Workflows.Steps;

public class RandomStepInput
{
    public int Min { get; set; }
    public int Max { get; set; }
}

public class RandomStep : StepBody
{
    public RandomStepInput Input { get; set; } = default!;
    public int CurrentNumber { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var random = new Random();
        CurrentNumber = random.Next(Input.Min, Input.Max + 1);
        return ExecutionResult.Next();
    }
}
