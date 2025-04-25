using Ktt.Workflows.Core;
using Ktt.Workflows.Implementation.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Ktt.Workflows.UnitTests.Workflows;

public class LeetMathWorkflowTests
{
    [Fact]
    public async Task LeetMathWorkflow_Should_Calculate_Correct_Result()
    {
        // Arrange
        var services = new ServiceCollection();

        // Use in-memory engine via our real DI extension
        services.AddWorkflowEngineImplementation(useInMemory: true);

        var sp = services.BuildServiceProvider();

        var host = sp.GetRequiredService<WorkflowHostedService>();

        try
        {
            await host.StartAsync(CancellationToken.None);

            var helper = sp.GetRequiredService<WorkflowEngineHelper>();

            var id = await helper.StartWorkflowAsync("LeetMath", new MathWorkflowData { CurrentNumber = 42 });
            await Task.Delay(120);

            while (true)
            {
                var status = await helper.GetWorkflowStatusAsync(id);

                Assert.True(!string.IsNullOrEmpty(status!.StatusTitle));

                if (status!.StatusTitle == "6/6 Finished")
                {
                    break;
                }

                await Task.Delay(20);
            }
        }
        finally
        {
            await host.StopAsync(CancellationToken.None);
        }
    }
}
