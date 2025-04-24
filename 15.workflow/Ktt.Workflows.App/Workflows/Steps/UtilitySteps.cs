using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.App.Workflows.Steps;

public class UpdateWorkflowStatusStep : StepBody
{
    public string Status { get; set; } = string.Empty;

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine($"Workflow Status: {Status}");
        return ExecutionResult.Next();
    }
}
