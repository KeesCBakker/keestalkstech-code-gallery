using Ktt.Workflows.Core.Models;
using WorkflowCore.Interface;

namespace Ktt.Workflows.Core.Workflows;
public abstract class WorkflowBase<TData> : IWorkflow<TData>
    where TData : IWorkflowDataWithState, new()
{
    public virtual string Id => WorkflowService.ResolveWorkflowId(GetType());

    public virtual int Version => 1;

    public abstract void Build(IWorkflowBuilder<TData> builder);
}
