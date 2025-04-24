using WorkflowCore.Interface;
using WorkflowCore.Models;
using Ktt.Workflows.App.Models;
using Ktt.Workflows.App.Workflows.Steps;

namespace Ktt.Workflows.App.Workflows;

public class GitHubWorkflowData
{
    public GitHubSettings Settings { get; set; } = default!;
    public string Status { get; set; } = string.Empty;
}

public class ProvisionGitHubWorkflow : IWorkflow<GitHubWorkflowData>
{
    public string Id => "GitHubProvisioningWorkflow";
    public int Version => 1;

    public void Build(IWorkflowBuilder<GitHubWorkflowData> builder)
    {
        builder
            .StartWith<ProvisionGitHubRepositoryStep>()
                .Input(step => step.Settings, (data, context) => data.Settings)
                .Name("Provision GitHub Repository")
            .Delay(data => TimeSpan.FromSeconds(2))
                .Name("Waiting for Provisioning")
            .Then<ProvisionGitHubRepositoryStep>()
                .Input(step => step.Settings, (data, context) => data.Settings)
                .Name("Finalize GitHub Repository");
    }
}
