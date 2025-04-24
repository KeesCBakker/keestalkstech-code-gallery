namespace Ktt.Workflows.App.Models.GitHub;

public record GitHubCreateBranchInput(
    string Repository,
    string BranchName
);
