using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add Workflow Core services with in-memory storage
        services.AddWorkflowEngineImplementation(options =>
        {
            // Use your actual connection string and key prefix
            options.UseRedisPersistence(@"localhost:6379", "workflow-core"); 
        });
    });

var host = builder.Build();

var starter = host.Services.GetRequiredService<WorkflowStarter>();
string? id = "cf7e293e-5278-4190-98cb-cf45f6ef773e";

// Keep the application running
var workflowHost = host.Services.GetRequiredService<WorkflowHostedService>();
_ = workflowHost.StartAsync(CancellationToken.None);

if (id == null)
{
    // Start the workflow host
    id = await starter.RunDelayWorkflow(new WorkflowDataWithState());
}

Console.WriteLine(id);

string status = "";

while (true)
{
    await Task.Delay(100);

    var xxx = await starter.GetWorkflowStatusAsync(id);
    if (xxx == null)
    {
        continue;
    }

    if (status != xxx.StatusTitle)
    {
        Console.WriteLine(xxx.StatusTitle);
        status = xxx.StatusTitle;
    }

    if (xxx.IsDone)
    {
        break;
    }
}
