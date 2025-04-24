using WorkflowCore.Interface;
using WorkflowCore.Models;
using Ktt.Workflows.App.Models;

namespace Ktt.Workflows.App.Workflows.Steps;

public class ProvisionJenkinsPipelinesStep : StepBody
{
    public JenkinsSettings Settings { get; set; } = default!;

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine($"Provisioning Jenkins pipelines: {Settings.GitHubRepoName}");
        // TODO: Implement actual Jenkins pipeline provisioning
        return ExecutionResult.Next();
    }
}
