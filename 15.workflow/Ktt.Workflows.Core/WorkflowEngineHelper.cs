using Ktt.Workflows.Core.Models;
using System.Security.Cryptography.X509Certificates;
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


        var failedStep = instance.ExecutionPointers.FirstOrDefault(x => x.Status == PointerStatus.Failed);
        if (failedStep != null)
        {
            data.State = WorkflowExecutionState.Failed;
        }


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

