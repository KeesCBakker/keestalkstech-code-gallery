
using Ktt.Workflows.Core.Models;

namespace Ktt.Workflows.Implementation.Models;

public class MathWorkflowData : WorkflowDataWithState, ICurrentNumber
{
    public int CurrentNumber { get; set; }
}
