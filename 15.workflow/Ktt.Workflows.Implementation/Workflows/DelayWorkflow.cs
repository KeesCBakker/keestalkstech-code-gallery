using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Core.Steps;
using WorkflowCore.Interface;

namespace Ktt.Workflows.Core.Workflows;

public class DelayWorkflow : IWorkflow<WorkflowDataWithState>
{
    public string Id => "Delay";

    public int Version => 1;

    public void Build(IWorkflowBuilder<WorkflowDataWithState> builder)
    {
        var cnt = 4;

        builder
            .Status($"1/{cnt} Initializing...")
            .Status($"2/{cnt} First Delay...")
            .Then<DelayStep>()
                .SafeInput(s => s.Delay, _ => TimeSpan.FromSeconds(2))
            .Status($"3/{cnt} Second Delay...")
            .Then<DelayStep>()
                .SafeInput(s => s.Delay, _ => TimeSpan.FromSeconds(2))
            .Finish($"4/{cnt} Finished");
    }
}
