namespace Ktt.Workflows.App.Models;

public record JenkinsSettings(
    string GitHubRepoName,
    string TeamName
);
