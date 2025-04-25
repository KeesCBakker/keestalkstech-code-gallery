using FluentAssertions;
using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Implementation.Workflows;
using Microsoft.Extensions.DependencyInjection;

namespace Ktt.Workflows.UnitTests.Workflows;

public class AddPostgresWorkflowTests
{
    [Fact]
    public async Task AddPostgresWorkflow_Should_CompleteSuccessfully_AndContainPassword()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddWorkflowEngineImplementation(useInMemory: true); // registers WorkflowHostedService

        using var provider = services.BuildServiceProvider();

        // Start workflow engine using WorkflowHostedService
        var hostedService = provider.GetRequiredService<WorkflowHostedService>();
        await hostedService.StartAsync(default);

        var engine = provider.GetRequiredService<WorkflowEngineHelper>();

        var data = new AddPostgresWorkflowData
        {
            Name = "checkout",
            Team = "payments",
            Environment = "dev",
            StorageInGb = 10,
            InstanceType = "db.t3.small"
        };

        // Act
        var workflowId = await engine.StartWorkflowAsync("AddPostgres", data);

        WorkflowStatusResult? result = null;

        while(true)
        {
            await Task.Delay(200);
            result = await engine.GetWorkflowStatusAsync(workflowId);
            if (result?.State is WorkflowExecutionState.Finished or WorkflowExecutionState.Failed)
            {
                break;
            }
        }

        // Assert
        result.Should().NotBeNull();
        result.State.Should().Be(WorkflowExecutionState.Finished);
        result.StatusTitle.Should().Be("6/6 Finished");
        result.Form.Should().ContainKey(WorkflowFormKeys.GeneratedPassword);
        result.Form[WorkflowFormKeys.GeneratedPassword].Should().NotBeNullOrWhiteSpace();

        await hostedService.StopAsync(default);
    }
}
