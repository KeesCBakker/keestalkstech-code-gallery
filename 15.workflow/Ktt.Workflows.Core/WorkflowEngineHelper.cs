using Ktt.Workflows.Core.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Core;

public class WorkflowService
{
    private readonly IWorkflowHost _workflowHost;
    private readonly IPersistenceProvider _persistenceProvider;

    public static string ResolveWorkflowId<TWorkflow>()
    {
        return ResolveWorkflowId(typeof(TWorkflow));
    }

    public static string ResolveWorkflowId(Type type)
    {
        return type.Name.Replace("Workflow", "");
    }

    public WorkflowService(
        IWorkflowHost workflowHost,
        IPersistenceProvider persistenceProvider)
    {
        _workflowHost = workflowHost;
        _persistenceProvider = persistenceProvider;
    }

    public Task<string> StartWorkflowAsync<TWorkflow>(object data)
    {
        var workflowId = ResolveWorkflowId<TWorkflow>();
        return StartWorkflowAsync(workflowId, data);
    }

    public async Task<string> StartWorkflowAsync(string workflowId, object data)
    {
        var id = await _workflowHost.StartWorkflow(workflowId, 1, data);
        return id;
    }

    public async Task<WorkflowStatusResult?> GetWorkflowStatusAsync(string id)
    {

        WorkflowInstance instance;

        try
        {
            instance = await _persistenceProvider.GetWorkflowInstance(id);
        }
        catch(Exception)
        {
            return null;
        }

        if (instance?.Data is not IWorkflowDataWithState data)
        {
            return null;
        }

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
        }

        return new WorkflowStatusResult
        {
            WorkflowId = id,
            StatusTitle = statusTitle,
            StatusDescription = statusDescription ?? string.Empty,
            LastExceptionMessage = data.LastException?.Message ?? string.Empty,
            State = data.State,
            Form = data.Form,
            Journal = data.Journal,
            WorkflowDefinitionId = instance.WorkflowDefinitionId
        };
    }
}

