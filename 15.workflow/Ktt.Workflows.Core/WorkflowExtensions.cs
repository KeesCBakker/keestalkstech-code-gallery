using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Core.Steps;
using System.Linq.Expressions;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Primitives;

namespace Ktt.Workflows.Core;

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

    public static IStepBuilder<TData, TStepBody> SafeInput<TData, TStepBody, TProperty>(
       this IStepBuilder<TData, TStepBody> builder,
       Expression<Func<TStepBody, TProperty>> property,
       Func<TData, TProperty> valueProvider
    )
       where TData : IWorkflowDataWithState
       where TStepBody : IStepBody
    {
        var stepName = typeof(TStepBody).Name;

        return builder
            .OnError(WorkflowErrorHandling.Terminate)
            .Input(property, data => Set(stepName, valueProvider, data));
    }

    public static TProperty Set<TData, TProperty>(
         string stepName,
         Func<TData, TProperty> valueProvider,
         TData data
     )
        where TData : IWorkflowDataWithState
    {
        try
        {
            var result = valueProvider(data);
            return result;
        }
        catch (Exception ex)
        {
            var nex = new Exception(
                $"Input mapping failed for {stepName}: {ex.Message}",
                ex);

            data.State = WorkflowExecutionState.Failed;
            data.StatusDescription = "Input mapping failed. Please retry and contact my admins if the error persists.";
            data.LastException = WorkflowExceptionInfo.From(nex);

            throw nex;
        }
    }

}

