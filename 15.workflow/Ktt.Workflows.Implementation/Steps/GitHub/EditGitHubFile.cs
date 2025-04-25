using Ktt.Workflows.Core.Steps;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps.GitHub;

public abstract class EditGitHubFile : SafeStep
{
    public GitHubFileDefinition Definition { get; set; } = default!;

    protected abstract string Edit(string currentContent);

    protected override Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
    {
        var d = Definition;

        var newContent = Edit("Old content");
        Journal(context, $"Edited file '{d.FilePath}' on branch '{d.Branch}' in repository '{d.Repository}'\n---\n{newContent}");

        // Simulate editing the file via GitHub API here

        return Next();
    }

    public class GitHubFileDefinition
    {
        public required string Repository { get; set; }

        public required string Branch { get; set; }

        public required string FilePath { get; set; }
    }
}
