using Ktt.Workflows.Core.Models;
using Ktt.Workflows.Core.Workflows;
using Ktt.Workflows.Implementation.Models;
using Ktt.Workflows.Implementation.Workflows;
using Ktt.Workflows.Implementation.Workflows.Maths;
using Microsoft.Extensions.DependencyInjection;
using WorkflowCore.Models;

namespace Ktt.Workflows.Core;

public static class WorkflowServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowEngineImplementation(
        this IServiceCollection services,
        Action<WorkflowOptions>? configureWorkflowEngine = null)
    {
        services.AddWorkflowEngine(configureWorkflowEngine);

        services.RegisterWorkflow<MathWorkflow, MathWorkflowData>();
        services.RegisterWorkflow<DivisionWorkflow, MathWorkflowData>();
        services.RegisterWorkflow<AddPostgresWorkflow, AddPostgresWorkflowData>();
        services.RegisterWorkflow<AddValkeyWorkflow, AddValkeyWorkflowData>();
        services.RegisterWorkflow<DelayWorkflow, WorkflowDataWithState>();

        services.AddTransient<WorkflowStarter>();

        return services;
    }
}
