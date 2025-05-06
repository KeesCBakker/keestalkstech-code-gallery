using Ktt.Workflows.Core;
using Ktt.Workflows.Implementation.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Ktt.Workflows.UnitTests.Workflows;

public class DivisionWorkflowTests
{
    [Fact]
    public async Task DivideWorkflow_Should_Fail()
    {
        // Arrange
        var services = new ServiceCollection();

        // Use in-memory engine via our real DI extension
        services.AddWorkflowEngineImplementation();

        var sp = services.BuildServiceProvider();
        var host = sp.GetRequiredService<WorkflowHostedService>();

        try
        {
            await host.StartAsync(CancellationToken.None);

            var helper = sp.GetRequiredService<WorkflowStarter>();

            var id = await helper.RunDivisionWorkflow(new MathWorkflowData { CurrentNumber = 42 });
            await Task.Delay(100);

            var status = await helper.GetWorkflowStatusAsync(id);
        }
        finally
        {
            await host.StopAsync(CancellationToken.None);
        }
    }
}
