using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkflowCore.Interface;
using Ktt.Workflows.App.Workflows;
using Ktt.Workflows.App.Models;
using Ktt.Workflows.App.Workflows.Steps;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add Workflow Core services with in-memory storage
        services.AddWorkflow();  // This will use in-memory storage by default

        // Register workflow steps
        services.AddTransient<UpdateWorkflowStatusStep>();
        services.AddTransient<ProvisionGitHubRepositoryStep>();
        services.AddTransient<ProvisionDockerHubRepositoryStep>();
        services.AddTransient<ProvisionJenkinsPipelinesStep>();

        // Register workflows
        services.AddTransient<ProvisionGitHubWorkflow>();
        services.AddTransient<ProvisionDockerHubWorkflow>();
        services.AddTransient<ProvisionJenkinsWorkflow>();
    });

var host = builder.Build();

// Start the workflow host
var workflowHost = host.Services.GetRequiredService<IWorkflowHost>();
workflowHost.RegisterWorkflow<ProvisionGitHubWorkflow, GitHubWorkflowData>();
workflowHost.RegisterWorkflow<ProvisionDockerHubWorkflow, DockerHubWorkflowData>();
workflowHost.RegisterWorkflow<ProvisionJenkinsWorkflow, JenkinsWorkflowData>();
workflowHost.Start();

// Create and run a sample workflow
var workflowController = host.Services.GetRequiredService<IWorkflowController>();
var workflowData = new GitHubWorkflowData
{
    Settings = new GitHubSettings(
        Name: "my-repo",
        Description: "My GitHub repository",
        Team: "my-team"
    )
};

var workflowId = await workflowController.StartWorkflow("GitHubProvisioningWorkflow", 1, workflowData);

Console.WriteLine($"Started workflow {workflowId}");

// Keep the application running
await host.RunAsync();
