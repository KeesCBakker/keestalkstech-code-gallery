using Ktt.Workflows.Core.Steps;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps.GitHub;

public class CreateGitHubBranch : SafeStep
{
    public GitHubBranchDefinition Definition { get; set; } = default!;

    protected override ExecutionResult Execute(IStepExecutionContext context)
    {
        var d = Definition;

        Journal(context, $"Created branch '{d.Branch}' in repository '{d.Repository}'");

        // Simulate GitHub API branch creation here
        return ExecutionResult.Next();
    }

    public class GitHubBranchDefinition
    {
        public required string Repository { get; set; }

        public required string Branch { get; set; }
    }
}

