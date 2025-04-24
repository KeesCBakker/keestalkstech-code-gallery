using Ktt.Workflows.Core.Steps;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps.GitHub;

public class CreateGitHubPr : SafeStep
{
    public GitHubPrDefinition Definition { get; set; } = default!;

    protected override ExecutionResult Execute(IStepExecutionContext context)
    {
        var d = Definition;

        Journal(context, $"Created PR for branch '{d.Branch}' in '{d.Repository}' with message: {d.Description}");

        // Simulate PR creation API call here
        return ExecutionResult.Next();
    }

    public record GitHubPrDefinition(
        string Repository,
        string Branch,
        string Description
    );
}
