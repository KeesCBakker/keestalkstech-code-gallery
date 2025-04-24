namespace Ktt.Workflows.App.Models.GitHub;

public record GitHubEditFileInput(
    string Repository,
    string BranchName,
    string FilePath,
    string Content
);
