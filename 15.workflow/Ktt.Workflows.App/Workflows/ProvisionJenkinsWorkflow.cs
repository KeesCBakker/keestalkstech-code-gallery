using WorkflowCore.Interface;
using WorkflowCore.Models;
using Ktt.Workflows.App.Models;
using Ktt.Workflows.App.Workflows.Steps;

namespace Ktt.Workflows.App.Workflows;

public class JenkinsWorkflowData
{
    public JenkinsSettings Settings { get; set; } = default!;
    public string Status { get; set; } = string.Empty;
}

public class ProvisionJenkinsWorkflow : IWorkflow<JenkinsWorkflowData>
{
    public string Id => "JenkinsProvisioningWorkflow";
    public int Version => 1;

    public void Build(IWorkflowBuilder<JenkinsWorkflowData> builder)
    {
        builder
            .StartWith<ProvisionJenkinsPipelinesStep>()
                .Input(step => step.Settings, (data, context) => data.Settings)
                .Name("Provision Jenkins Pipelines")
            .Delay(data => TimeSpan.FromSeconds(2))
                .Name("Waiting for Provisioning")
            .Then<ProvisionJenkinsPipelinesStep>()
                .Input(step => step.Settings, (data, context) => data.Settings)
                .Name("Finalize Jenkins Pipelines");
    }
}
