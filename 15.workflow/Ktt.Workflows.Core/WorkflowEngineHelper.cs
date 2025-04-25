using Ktt.Workflows.Core.Models;
using WorkflowCore.Interface;

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

    public async Task<string> StartWorkflowAsync<TData>(string workflowId, TData data)
        where TData : class, new()
    {
        var id = await _workflowHost.StartWorkflow(workflowId, 1, data);
        return id;
    }

    public async Task<WorkflowStatusResult?> GetWorkflowStatusAsync(string id)
    {
        var instance = await _persistenceProvider.GetWorkflowInstance(id);
        if (instance?.Data is not IWorkflowDataWithState data)
            return null;

        var statusTitle = data.StatusTitle;
        var statusDescription = data.StatusDescription;

        if (data.State == WorkflowExecutionState.Failed)
        {
            // Always update the title for failed state
            var idx = statusTitle.LastIndexOf("...", StringComparison.Ordinal);
            if (idx != -1)
            {
                statusTitle = statusTitle[..idx] + " failed";
            }
            else
            {
                statusTitle += " failed";
            }

            // Append exception to description if available
            if (data.LastException != null)
            {
                var message = $"failed: {data.LastException.Message}";
                statusDescription = string.IsNullOrWhiteSpace(statusDescription)
                    ? message
                    : $"{statusDescription} ({message})";
            }
        }

        return new WorkflowStatusResult
        {
            WorkflowId = id,
            StatusTitle = statusTitle,
            StatusDescription = statusDescription ?? string.Empty,
            State = data.State,
            Form = data.Form,
            Journal = data.Journal
        };
    }
}

