using Ktt.Workflows.Core.Steps;
using WorkflowCore.Interface;
using WorkflowCore.Primitives;

namespace Ktt.Workflows.Core.Workflows;

public static class WorkflowExtensions
{
    public static IStepBuilder<TData, StatusStep> Status<TData, TStepBody>(this IStepBuilder<TData, TStepBody> wflw, string status) where TStepBody : IStepBody
    {
        return wflw
            .Then<StatusStep>()
            .Input(s => s.Status, _ => status);
    }

    public static IStepBuilder<TData, StatusStep> Status<TData>(this IWorkflowBuilder<TData> builder, string status)
    {
        return builder
            .StartWith<StatusStep>()
            .Input(s => s.Status, _ => status);
    }

    public static IStepBuilder<TData, Delay> Delay<TData, TStepBody>(this IStepBuilder<TData, TStepBody> wflw, TimeSpan ts) where TStepBody : IStepBody
    {
        return wflw
            .Then<Delay>()
            .Input(s => s.Period, _ => ts);
    }

}

