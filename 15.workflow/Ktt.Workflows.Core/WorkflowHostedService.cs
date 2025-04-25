using Microsoft.Extensions.Hosting;
using WorkflowCore.Interface;

namespace Ktt.Workflows.Core;

public class WorkflowHostedService : IHostedService
{
    private readonly IWorkflowHost _host;
    private readonly IEnumerable<WorkflowHostConfigurator> _configurators;

    public WorkflowHostedService(
        IWorkflowHost host,
        IEnumerable<WorkflowHostConfigurator> configurators)
    {
        _host = host;
        _configurators = configurators;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var items = _configurators.ToList();
        foreach (var action in items)
        {
            action(_host);
        }

        _host.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _host.Stop();
        return Task.CompletedTask;
    }
}

public delegate void WorkflowHostConfigurator(IWorkflowHost host);
