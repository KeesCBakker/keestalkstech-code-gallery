using WorkflowCore.Interface;
using WorkflowCore.Models;
using Ktt.Workflows.App.Workflows.Steps;

namespace Ktt.Workflows.App.Workflows;

public class MathWorkflow : IWorkflow<NumberWorkflowData>
{
    public string Id => "MathWorkflow";
    public int Version => 1;

    public void Build(IWorkflowBuilder<NumberWorkflowData> builder)
    {
        builder
            .StartWith<RandomStep>()
                .Input(step => step.Input, (data, context) => new RandomStepInput { Min = 1, Max = 10 })
                .Output(data => data.CurrentNumber, step => step.CurrentNumber)
                .Name("Generate Random Number")
            .Then<MultiplyStep>()
                .Input(step => step.Input, (data, context) => new MultiplyStepInput { Multiplier = 8 })
                .Input(step => step.CurrentNumber, (data, context) => data.CurrentNumber)
                .Output(data => data.CurrentNumber, step => step.CurrentNumber)
                .Name("Multiply by 8")
            .Then<AddStep>()
                .Input(step => step.Input, (data, context) => new AddStepInput { Value = 2 })
                .Input(step => step.CurrentNumber, (data, context) => data.CurrentNumber)
                .Output(data => data.CurrentNumber, step => step.CurrentNumber)
                .Name("Add 2");
    }
}
