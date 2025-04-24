using Ktt.Workflows.Core.Steps;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using WorkflowCore.Interface;

namespace Ktt.Workflows.Core;

public static class AssemblyScanner
{
    public static IServiceCollection AddWorkflowEngine(this IServiceCollection services, bool useInMemory = false)
    {
        services.AddLogging();

        services.AddWorkflow(cfg =>
        {
            if (!useInMemory)
            {
                cfg.UseSqlite("Data Source=workflow.db;", true);
            }
        });

        RegisterAllSteps(services, typeof(SafeStep).Assembly);

        services.AddSingleton<WorkflowHostedService>();
        services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<WorkflowHostedService>());

        services.AddTransient<WorkflowEngineHelper>();

        return services;
    }

    public static void RegisterAllSteps(this IServiceCollection services, Assembly assembly)
    {
        var stepTypes = assembly.GetTypes()
            .Where(t => typeof(IStepBody).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var stepType in stepTypes)
        {
            services.AddTransient(stepType);
        }
    }

    public static void RegisterAllWorkflows(this IServiceCollection services, Assembly assembly)
    {
        var workflowTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<AutoRegisterWorkflowAttribute>() != null)
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IWorkflow<>)))
            .ToList();

        foreach (var type in workflowTypes)
        {
            services.AddTransient(type);
        }
    }

    public static IServiceCollection RegisterWorkflow<TWorkflow, TData>(this IServiceCollection services)
        where TWorkflow : class, IWorkflow<TData>
        where TData : class, new()
    {
        services.AddTransient<TWorkflow>();

        services.AddSingleton<WorkflowHostConfigurator>(host =>
        {
            host.RegisterWorkflow<TWorkflow, TData>();
        });

        return services;
    }
}
