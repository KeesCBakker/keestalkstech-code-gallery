namespace Ktt.Workflows.App.Models;

public record GitHubWorkflowSettings(
    string Name,
    string Description,
    string Team,
    bool ProvisionDockerHub,
    bool ProvisionJenkinsPipelines
) : GitHubSettings(Name, Description, Team);
