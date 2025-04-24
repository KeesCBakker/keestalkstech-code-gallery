using WorkflowCore.Interface;
using WorkflowCore.Models;
using Ktt.Workflows.App.Models;

namespace Ktt.Workflows.App.Workflows.Steps;

public class ProvisionDockerHubRepositoryStep : StepBody
{
    public DockerHubSettings Settings { get; set; } = default!;

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine($"Provisioning DockerHub repository: {Settings.GitHubRepoName}");
        // TODO: Implement actual DockerHub repository provisioning
        return ExecutionResult.Next();
    }
}
