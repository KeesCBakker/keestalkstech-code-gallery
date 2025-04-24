using WorkflowCore.Interface;
using WorkflowCore.Models;
using Ktt.Workflows.App.Models;

namespace Ktt.Workflows.App.Workflows.Steps;

public class ProvisionApplicationSetStep : StepBody
{
    public ApplicationSetSettings Settings { get; set; } = default!;

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine($"Provisioning Application Set: {Settings.Environment}");
        // TODO: Implement actual Application Set provisioning
        return ExecutionResult.Next();
    }
}
