using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Workflows;
using Ktt.Workflows.Implementation.Models;
using Ktt.Workflows.Implementation.Steps.Maths;
using WorkflowCore.Interface;
using static Ktt.Workflows.Implementation.Steps.Maths.MathStep;

namespace Ktt.Workflows.Implementation.Workflows.Maths;

public sealed class MathWorkflow : WorkflowBase<MathWorkflowData>
{
    public override void Build(IWorkflowBuilder<MathWorkflowData> builder)
    {
        var cnt = 5;

        builder
            .Status($"1/{cnt} Initializing...")
            .Then<MathStep>()
                .SafeInput(s => s.Operator, _ => MathStepOperator.Add)
                .SafeInput(s => s.Operand, _ => 1)
            .Status($"2/{cnt} Add...")
            .Then<MathStep>()
                .SafeInput(s => s.Operator, _ => MathStepOperator.Multiply)
                .SafeInput(s => s.Operand, _ => 3)
            .Status($"3/{cnt} Multiply...")
            .Then<MathStep>()
                .SafeInput(s => s.Operator, _ => MathStepOperator.Add)
                .SafeInput(s => s.Operand, _ => -3)
            .Status($"4/{cnt} Add...")
            .Then<MathStep>()
                .SafeInput(s => s.Operator, _ => MathStepOperator.Multiply)
                .SafeInput(s => s.Operand, _ => 7)
            .Finish($"5/{cnt} Finished");
    }
}
