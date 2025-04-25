using Ktt.Workflows.Implementation.Models;
using Ktt.Workflows.Implementation.Steps.Math;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Core.Workflows;

[AutoRegisterWorkflow]
public class DivisionWorkflow : IWorkflow<MathWorkflowData>
{
    public string Id => "DivisionWorkflow";

    public int Version => 1;

    public void Build(IWorkflowBuilder<MathWorkflowData> builder)
    {
        var cnt = 2;

        builder
            .Status($"1/{cnt} Dividing...")
            .Then<DivideStep>()
                .Input(s => s.Divisor, _ => 0)
            .Status($"2/{cnt} Finished")
            .OnError(WorkflowErrorHandling.Terminate);
    }
}
