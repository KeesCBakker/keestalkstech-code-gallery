using Ktt.Workflows.Core;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps.GitHub;

public class CreateGitHubPr : SafeStep
{
    public required GitHubPrDefinition Definition { get; set; }

    protected override Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
    {
        var d = Definition;

        var prId = $"{Guid.NewGuid().ToString()[..8]}";
        var prUrl = $"https://github.com/{d.Repository}/pull/{prId}";

        Journal(context, $"Created pull request in {d.Repository}: {prUrl}");

        Data.SetFormValue(WorkflowFormKeys.GitHubPullRequestId, prId);
        Data.SetFormValue(WorkflowFormKeys.GitHubPullRequestUrl, prUrl);

        return Next();
    }

    public class GitHubPrDefinition
    {
        public required string Repository { get; set; }
        public required string Branch { get; set; }
        public required string Description { get; set; }
    }
}
