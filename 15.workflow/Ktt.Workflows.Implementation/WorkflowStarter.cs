using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Core.Workflows;
using Ktt.Workflows.Implementation.Models;
using Ktt.Workflows.Implementation.Workflows;
using Ktt.Workflows.Implementation.Workflows.Maths;
using Ktt.Workflows.Implementation.Workflows.Provisioning;
using WorkflowCore.Interface;

namespace Ktt.Workflows.Core;

public class WorkflowStarter : WorkflowService
{
    public WorkflowStarter(IWorkflowHost workflowHost, IPersistenceProvider persistenceProvider) : base(workflowHost, persistenceProvider)
    {
    }

    public Task<string> RunAtlasPostgresWorkflow(AddAtlasPostgresWorkflowData data)
        => StartWorkflowAsync<AddAtlasPostgresWorkflowData>(data);

    public Task<string> RunDivisionWorkflow(MathWorkflowData data)
        => StartWorkflowAsync<MathWorkflow>(data);

    public Task<string> RunValkeyWorkflow(AddValkeyWorkflowData data)
        => StartWorkflowAsync<AddValkeyWorkflow>(data);

    public Task<string> RunPostgresWorkflow(AddPostgresWorkflowData data)
        => StartWorkflowAsync<AddPostgresWorkflow>(data);

    public Task<string> RunMathWorkflow(MathWorkflowData data)
        => StartWorkflowAsync<MathWorkflow>(data);

    public Task<string> RunDelayWorkflow(WorkflowDataWithState data)
        => StartWorkflowAsync<DelayWorkflow>(data);
}
