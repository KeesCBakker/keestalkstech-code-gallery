using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps.Resources;
public class EnsureAtlasApplicationDirectoryStep : SafeStep
{
    public required string ApplicationName { get; set; }

    public required string Environment { get; set; }

    public required string Repository { get; set; }

    protected override ExecutionResult Execute(IStepExecutionContext context)
    {
        // Simulated logic — in reality this might check a GitHub repo for a path
        Journal(context, $"Ensured application directory for '{ApplicationName}' in environment '{Environment}' in repository '{Repository}'.");

        return ExecutionResult.Next();
    }
}
