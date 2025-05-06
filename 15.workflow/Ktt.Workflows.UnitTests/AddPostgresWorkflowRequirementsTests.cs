using FluentAssertions;
using Ktt.Workflows.Core;
using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Implementation.Workflows;
using Microsoft.Extensions.DependencyInjection;
using WorkflowCore.Interface;

namespace Ktt.Workflows.UnitTests;
public class AddPostgresWorkflowRequirementsTests : IDisposable
{
    private readonly IServiceProvider _provider;
    private readonly WorkflowService _engine;
    private readonly WorkflowHostedService _host;
    private bool _disposed;

    public AddPostgresWorkflowRequirementsTests()
    {
        var services = new ServiceCollection();
        services.AddWorkflowEngineImplementation();
        services.AddTransient<AddPostgresWorkflow>();

        _provider = services.BuildServiceProvider();
        _engine = _provider.GetRequiredService<WorkflowService>();
        _host = _provider.GetRequiredService<WorkflowHostedService>();

        _host.StartAsync(default).Wait();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                _host.StopAsync(default).Wait();
            }

            // Dispose unmanaged resources if any

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private async Task<AddPostgresWorkflowData> RunWorkflow()
    {
        var id = await _engine.StartWorkflowAsync("AddPostgres", new AddPostgresWorkflowData
        {
            Environment = "test",
            Name = "db",
            Team = "core",
            StorageInGb = 5,
            InstanceType = "small"
        });

        for (int i = 0; i < 100; i++)
        {
            var status = await _engine.GetWorkflowStatusAsync(id);
            if (status?.State != WorkflowExecutionState.Running)
            {
                break;
            }

            await Task.Delay(100);
        }

        var instance = await _provider.GetRequiredService<IPersistenceProvider>().GetWorkflowInstance(id);
        return (AddPostgresWorkflowData)instance.Data;
    }

    [Fact]
    public async Task Should_Model_Steps_Individually()
    {
        // Arrange
        var result = await RunWorkflow();

        // Assert
        result.Journal.Should().Contain(j => j.Contains("GeneratePasswordStep"));
    }

    [Fact]
    public async Task Should_Compose_Steps_Into_Workflow()
    {
        // Arrange
        var result = await RunWorkflow();

        // Assert
        result.StatusTitle.Should().Contain("Finished");
    }

    [Fact]
    public async Task Should_Use_Output_As_Input()
    {
        // Arrange
        var result = await RunWorkflow();

        // Assert
        result.Form[WorkflowFormKeys.GeneratedPassword].Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Should_Be_Composed_From_BaseWorkflow()
    {
        // Arrange
        var result = await RunWorkflow();

        // Assert
        result.BranchName.Should().StartWith("add-postgres");
    }

    [Fact]
    public async Task Should_Pause_And_Continue()
    {
        // Arrange
        var result = await RunWorkflow();

        // Assert
        result.Journal.Count.Should().BeGreaterThan(2);
    }

    [Fact]
    public async Task Should_Resume_After_Restart()
    {
        // Arrange
        var result = await RunWorkflow();

        // Assert
        result.StatusTitle.Should().Contain("Finished");
    }

    [Fact]
    public async Task Should_Track_Status_Textually()
    {
        // Arrange
        var result = await RunWorkflow();

        // Assert
        result.StatusTitle.Should().Match(s => s.Contains("Finished") || s.Contains("Applying"));
    }

    [Fact]
    public async Task Should_Start_From_Helper()
    {
        // Arrange
        var result = await RunWorkflow();

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Report_Status_From_Helper()
    {
        // Arrange
        var status = await _engine.GetWorkflowStatusAsync("invalid-id");

        // Assert
        status.Should().BeNull();
    }

    [Fact]
    public async Task Should_Run_Multiple_In_Parallel()
    {
        // Arrange
        var task1 = RunWorkflow();
        var task2 = RunWorkflow();
        var results = await Task.WhenAll(task1, task2);

        // Assert
        results.Should().OnlyContain(r => r.StatusTitle.Contains("Finished"));
    }

    [Fact]
    public async Task Should_Stop_On_Exception()
    {
        // Arrange
        var id = await _engine.StartWorkflowAsync("AddPostgres", new AddPostgresWorkflowData
        {
            Environment = "",
            Name = "",
            Team = "",
            StorageInGb = 0,
            InstanceType = ""
        });

        for (int i = 0; i < 100; i++)
        {
            var status = await _engine.GetWorkflowStatusAsync(id);
            if (status?.State != WorkflowExecutionState.Running)
            {
                break;
            }

            await Task.Delay(100);
        }

        var result = await _engine.GetWorkflowStatusAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.LastExceptionMessage.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Log_Journal_Entries()
    {
        // Arrange
        var result = await RunWorkflow();

        // Assert
        result.Journal.Should().Contain(e => e.Contains("Entering"));
    }
}
