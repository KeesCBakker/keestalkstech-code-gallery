using Ktt.Workflows.Implementation.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps.Maths;

public class MathStep : SafeStep
{
    public int Operand { get; set; }

    public MathStepOperator Operator { get; set; }

    protected override Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
    {
        if (context.Workflow.Data is not ICurrentNumber data)
        {
            throw new InvalidOperationException(
                $"Workflow data must be of type {nameof(MathWorkflowData)} to use {nameof(MathStep)}.");
        }

        switch (Operator)
        {
            case MathStepOperator.Add:
                data.CurrentNumber += Operand;
                break;
            case MathStepOperator.Subtract:
                data.CurrentNumber -= Operand;
                break;
            case MathStepOperator.Multiply:
                data.CurrentNumber *= Operand;
                break;
        }

        data.Form["Result"] = data.CurrentNumber.ToString();

        return Next();
    }

    public enum MathStepOperator
    {
        Add,
        Multiply,
        Subtract
    }
}
