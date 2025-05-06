using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Workflows;
using Ktt.Workflows.Implementation.Models;
using Ktt.Workflows.Implementation.Steps.Maths;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Workflows.Maths;

public class DivisionWorkflow : WorkflowBase<MathWorkflowData>
{
    public sealed override void Build(IWorkflowBuilder<MathWorkflowData> builder)
    {
        var cnt = 2;

        builder
            .Status($"1/{cnt} Dividing...")
            .Then<DivideStep>()
                .Input(s => s.Divisor, _ => 0)
               .OnError(WorkflowErrorHandling.Terminate)
            .Finish($"2/{cnt} Finished");
    }
}
