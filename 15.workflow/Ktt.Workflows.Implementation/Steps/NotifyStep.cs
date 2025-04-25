using Ktt.Workflows.Core.Services;
using Ktt.Workflows.Core.Steps;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps;

public class NotifyStep(INotificationService notifier) : SafeStep
{
    public string Message { get; set; } = string.Empty;

    protected override Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
    {
        notifier.Send(Message);

        return Next();
    }
}
