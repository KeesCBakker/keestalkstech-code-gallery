using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps.GitHub;

public class CreateGitHubBranch : SafeStep
{
    public GitHubBranchDefinition Definition { get; set; } = default!;

    protected override Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
    {
        var d = Definition;

        Journal(context, $"Created branch '{d.Branch}' in repository '{d.Repository}'");

        // Simulate GitHub API branch creation here

        return Next();
    }

    public class GitHubBranchDefinition
    {
        public required string Repository { get; set; }

        public required string Branch { get; set; }
    }
}

