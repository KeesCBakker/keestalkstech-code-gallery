using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkflowCore.Interface;
using Ktt.Workflows.App.Models;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add Workflow Core services with in-memory storage
        services.AddWorkflow();  // This will use in-memory storage by default

    });

var host = builder.Build();

// Start the workflow host
var workflowHost = host.Services.GetRequiredService<IWorkflowHost>();
workflowHost.Start();

// Create and run a sample workflow
var workflowController = host.Services.GetRequiredService<IWorkflowController>();

// Keep the application running
await host.RunAsync();
