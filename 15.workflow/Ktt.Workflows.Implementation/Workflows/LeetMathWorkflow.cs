using Ktt.Workflows.Implementation.Models;
using Ktt.Workflows.Implementation.Steps;
using Ktt.Workflows.Implementation.Steps.Math;
using WorkflowCore.Interface;
using static Ktt.Workflows.Implementation.Steps.Math.MathStep;

namespace Ktt.Workflows.Core.Workflows;

[AutoRegisterWorkflow]
public class LeetMathWorkflow : IWorkflow<MathWorkflowData>
{
    public string Id => "LeetMath";

    public int Version => 1;

    public void Build(IWorkflowBuilder<MathWorkflowData> builder)
    {
        var cnt = 6;

        builder
            .Status($"1/{cnt} Initializing...")
            .Then<MathStep>()
                .Input(s => s.Operator, _ => MathStepOperator.Add)
                .Input(s => s.Operand, _ => 1)
            .Status($"2/{cnt} Add...")
            .Then<MathStep>()
                .Input(s => s.Operator, _ => MathStepOperator.Multiply)
                .Input(s => s.Operand, _ => 3)
            .Status($"3/{cnt} Multiply...")
            .Then<MathStep>()
                .Input(s => s.Operator, _ => MathStepOperator.Add)
                .Input(s => s.Operand, _ => -3)
            .Status($"4/{cnt} Add...")
            .Then<MathStep>()
                .Input(s => s.Operator, _ => MathStepOperator.Multiply)
                .Input(s => s.Operand, _ => 7)
            .Status($"5/{cnt} Notify...")
            .Then<NotifyStep>()
                .Input(s => s.Message, data => $"Current number is {data.CurrentNumber}")
            .Status($"6/{cnt} Finished");
    }
}
