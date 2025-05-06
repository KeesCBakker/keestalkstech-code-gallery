using Ktt.Workflows.Core.Steps;
using Ktt.Workflows.Implementation.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps.Maths;

public class DivideStep : SafeStep
{
    public int Divisor { get; set; }

    protected override Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
    {
        if (context.Workflow.Data is not MathWorkflowData data)
        {
            throw new InvalidOperationException("Expected LeetMathWorkflowData");
        }

        if (Divisor <= 0)
        {
            throw new InvalidOperationException("Divisor must be greater than 0");
        }

        data.CurrentNumber = data.CurrentNumber / Divisor;

        return Next();
    }
}
