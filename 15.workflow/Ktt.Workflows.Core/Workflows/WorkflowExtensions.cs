using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Core.Steps;
using WorkflowCore.Interface;
using WorkflowCore.Primitives;

namespace Ktt.Workflows.Core.Workflows;

public static class WorkflowExtensions
{
    public static IStepBuilder<TData, StatusStep> Status<TData, TStepBody>(
        this IStepBuilder<TData, TStepBody> wflw,
        string statusTitle,
        string? statusDescription = null,
        WorkflowExecutionState? state = null
    ) where TStepBody : IStepBody
    {
        var builder = wflw
            .Then<StatusStep>()
            .Input(s => s.StatusTitle, _ => statusTitle)
            .Input(s => s.StatusDescription, _ => statusDescription ?? string.Empty)
            .Input(s => s.State, _ => state);

        return builder;
    }

    public static IStepBuilder<TData, StatusStep> Status<TData, TStepBody>(
        this IStepBuilder<TData, TStepBody> wflw,
        string statusTitle,
        WorkflowExecutionState state
    ) where TStepBody : IStepBody
    {
        return wflw.Status(statusTitle, null, state);
    }


    public static IStepBuilder<TData, StatusStep> Status<TData>(this IWorkflowBuilder<TData> builder, string status)
    {
        return builder
            .StartWith<StatusStep>()
            .Input(s => s.StatusTitle, _ => status);
    }

    public static IStepBuilder<TData, StatusStep> Finish<TData, TStepBody>(
        this IStepBuilder<TData, TStepBody> wflw,
        string statusTitle
    ) where TStepBody : IStepBody
    {
        return wflw.Status(statusTitle, null, WorkflowExecutionState.Finished);
    }

    public static IStepBuilder<TData, Delay> Delay<TData, TStepBody>(this IStepBuilder<TData, TStepBody> wflw, TimeSpan ts) where TStepBody : IStepBody
    {
        return wflw
            .Then<Delay>()
            .Input(s => s.Period, _ => ts);
    }

}

