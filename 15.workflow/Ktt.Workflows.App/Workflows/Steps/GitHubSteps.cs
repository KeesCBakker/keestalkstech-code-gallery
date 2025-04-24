using WorkflowCore.Interface;
using WorkflowCore.Models;
using Ktt.Workflows.App.Models;
using System;

namespace Ktt.Workflows.App.Workflows.Steps;

public class ProvisionGitHubRepositoryStep : StepBody
{
    public GitHubSettings Settings { get; set; } = default!;

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        if (Settings == null)
            throw new InvalidOperationException("GitHub settings are required");

        Console.WriteLine($"Provisioning GitHub repository: {Settings.Name}");
        // TODO: Implement actual GitHub repository provisioning

        // Add a random delay between 5-16 seconds
        var random = new Random();
        var delaySeconds = random.Next(5, 17);
        return ExecutionResult.Sleep(TimeSpan.FromSeconds(delaySeconds), new { delaySeconds });
    }
}

public class CreateBranchOnGitHubStep : StepBody
{
    public GitHubSettings Settings { get; set; } = default!;
    public string BranchName { get; set; } = string.Empty;

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        if (Settings == null)
            throw new InvalidOperationException("GitHub settings are required");

        Console.WriteLine($"Creating branch '{BranchName}' in GitHub repository: {Settings.Name}");
        // TODO: Implement actual branch creation
        return ExecutionResult.Next();
    }
}

public class EditFileOnGitHubStep : StepBody
{
    public GitHubSettings Settings { get; set; } = default!;
    public string FilePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        if (Settings == null)
            throw new InvalidOperationException("GitHub settings are required");

        Console.WriteLine($"Editing file '{FilePath}' in GitHub repository: {Settings.Name}");
        // TODO: Implement actual file editing
        return ExecutionResult.Next();
    }
}
