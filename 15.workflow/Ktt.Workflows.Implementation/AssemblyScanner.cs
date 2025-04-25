using Ktt.Workflows.Core.Workflows;
using Ktt.Workflows.Implementation.Models;
using Ktt.Workflows.Implementation.Workflows;
using Microsoft.Extensions.DependencyInjection;

namespace Ktt.Workflows.Core;

public static class WorkflowServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowEngineImplementation(this IServiceCollection services, bool useInMemory = false)
    {
        services.AddWorkflowEngine(useInMemory);

        services.RegisterWorkflow<LeetMathWorkflow, MathWorkflowData>();
        services.RegisterWorkflow<DivisionWorkflow, MathWorkflowData>();
        services.RegisterWorkflow<AddPostgresWorkflow, AddPostgresWorkflowData>();
        services.RegisterWorkflow<AddValkeyWorkflow, AddValkeyWorkflowData>();

        return services;
    }
}
