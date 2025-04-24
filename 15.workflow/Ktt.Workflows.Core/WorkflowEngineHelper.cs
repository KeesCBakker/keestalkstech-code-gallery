using Ktt.Workflows.Core.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Core;

public class WorkflowEngineHelper
{
    private readonly IWorkflowHost _workflowHost;
    private readonly IPersistenceProvider _persistenceProvider;

    public WorkflowEngineHelper(
        IWorkflowHost workflowHost,
        IPersistenceProvider persistenceProvider)
    {
        _workflowHost = workflowHost;
        _persistenceProvider = persistenceProvider;
    }

    public async Task<string> StartWorkflow<TData>(string workflowId, TData data)
        where TData : class, new()
    {
        var id = await _workflowHost.StartWorkflow(workflowId, 1, data);
        return id;
    }

    public async Task<WorkflowStatusResult> GetWorkflowStatusAsync(string id)
    {
        var instance = await _persistenceProvider.GetWorkflowInstance(id);

        if (instance?.Data is not IWorkflowDataWithState data)
        {
            return new WorkflowStatusResult(id, "Unknown or invalid workflow", WorkflowExecutionState.Failed);
        }

        var text = GetStatusText(data);
        var state = GetExecutionState(instance, data);

        return new WorkflowStatusResult(id, text, state);
    }

    private static string GetStatusText(IWorkflowDataWithState data)
    {
        var status = data.Status;

        if (data.LastException != null)
        {
            var idx = status.LastIndexOf("...", StringComparison.Ordinal);
            if (idx != -1)
            {
                status = status[..idx] + $" failed: {data.LastException.Message}";
            }
            else
            {
                status += $" failed: {data.LastException.Message}";
            }
        }

        return status;
    }

    private static WorkflowExecutionState GetExecutionState(WorkflowInstance instance, IWorkflowDataWithState data)
    {
        if (data.LastException != null)
        {
            return WorkflowExecutionState.Failed;
        }

        return instance.Status switch
        {
            WorkflowStatus.Complete => WorkflowExecutionState.Finished,
            WorkflowStatus.Terminated => WorkflowExecutionState.Failed,
            WorkflowStatus.Suspended => WorkflowExecutionState.Failed,
            _ => WorkflowExecutionState.Running
        };
    }
}

