using WorkflowCore.Interface;
using WorkflowCore.Models;
using Ktt.Workflows.App.Models;

namespace Ktt.Workflows.App.Workflows.Steps;

public class ProvisionPostgresDatabaseStep : StepBody
{
    public PostgresSettings Settings { get; set; } = default!;

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine($"Provisioning Postgres database: {Settings.Name}");
        // TODO: Implement actual Postgres database provisioning
        return ExecutionResult.Next();
    }
}
