using Ktt.Workflows.Core.Steps;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps.GitHub;

public class AddGitHubComment : SafeStep
{
    public GitHubCommentDefinition Definition { get; set; } = default!;

    protected override ExecutionResult Execute(IStepExecutionContext context)
    {
        var d = Definition;

        Journal(context, $"Added comment to PR {d.PullRequestId} in {d.Repository}: {d.Comment}");

        // Simulate API call to GitHub here
        return ExecutionResult.Next();
    }

    public class GitHubCommentDefinition
    {
        public required string Repository { get; set; }

        public required string PullRequestId { get; set; }

        public required string Comment { get; set; }
    }
}
