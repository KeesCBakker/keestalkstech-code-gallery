using Ktt.Workflows.Core.Models;

namespace Ktt.Workflows.Implementation.Workflows.Provisioning;

public interface IInstanceDefinition : IWorkflowDataWithState
{
    string Environment { get; }

    string Name { get; }

    string BranchName { get; }
}
