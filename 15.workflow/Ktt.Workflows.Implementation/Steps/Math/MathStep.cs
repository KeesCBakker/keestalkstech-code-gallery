using Ktt.Workflows.Core.Steps;
using Ktt.Workflows.Implementation.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps.Math;

public class MathStep : SafeStep
{
    public int Operand { get; set; }

    public MathStepOperator Operator { get; set; }

    protected override ExecutionResult Execute(IStepExecutionContext context)
    {
        if (context.Workflow.Data is not ICurrentNumber data)
        {
            throw new InvalidOperationException(
                $"Workflow data must be of type {nameof(MathWorkflowData)} to use {nameof(MathStep)}.");
        }

        var stepState = data.GetStepState<StepProgress>(context);
        stepState.OriginalNumber = stepState.OriginalNumber ?? data.CurrentNumber;

        bool shouldContinue = stepState.Progress < Operand;

        if (shouldContinue)
        {
            switch (Operator)
            {
                case MathStepOperator.Add:
                    data.CurrentNumber += 1;
                    break;
                case MathStepOperator.Subtract:
                    data.CurrentNumber -= 1;
                    break;
                case MathStepOperator.Multiply:
                    data.CurrentNumber += stepState.OriginalNumber ?? 0;
                    break;
            }

            stepState.Progress += 1;
            data.SetStepState(context, stepState);

            return ExecutionResult.Sleep(TimeSpan.FromMilliseconds(10), null);
        }

        return ExecutionResult.Next();
    }

    private sealed class StepProgress
    {
        public int Progress { get; set; }

        public int? OriginalNumber { get; set; }
    }

    public enum MathStepOperator
    {
        Add,
        Multiply,
        Subtract
    }
}
