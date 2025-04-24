using Ktt.Workflows.App.Models;
using Ktt.Workflows.App.Workflows.Steps;
using WorkflowCore.Interface;

namespace Ktt.Workflows.App.Workflows;

public class DockerHubWorkflowData
{
    public DockerHubSettings Settings { get; set; } = default!;
    public string Status { get; set; } = string.Empty;
}

public class ProvisionDockerHubWorkflow : IWorkflow<DockerHubWorkflowData>
{
    public string Id => "DockerHubProvisioningWorkflow";
    public int Version => 1;

    public void Build(IWorkflowBuilder<DockerHubWorkflowData> builder)
    {
        builder
            .StartWith<ProvisionDockerHubRepositoryStep>()
                .Input(step => step.Settings, (data, context) => data.Settings)
                .Name("Provision Docker Hub Repository")
            .Delay(data => TimeSpan.FromSeconds(2))
                .Name("Waiting for Provisioning")
            .Then<ProvisionDockerHubRepositoryStep>()
                .Input(step => step.Settings, (data, context) => data.Settings)
                .Name("Finalize Docker Hub Repository");
    }
}
