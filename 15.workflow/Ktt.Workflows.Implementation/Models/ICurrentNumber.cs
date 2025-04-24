using Ktt.Workflows.Core.Models;

namespace Ktt.Workflows.Implementation.Models;

public interface ICurrentNumber : IWorkflowDataWithState
{
    public int CurrentNumber { get; set; }
}
