namespace Ktt.Workflows.App.Models.GitHub;

public record EditFileContents(
    string Contents,
    string CommitMessage
);