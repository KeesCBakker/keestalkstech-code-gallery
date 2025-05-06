using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Core;

public static class AssemblyScanner
{
    public static IServiceCollection AddWorkflowEngine(
        this IServiceCollection services,
        Action<WorkflowOptions>? configureWorkflowEngine = null)
    {
        RegisterAllSteps(services, typeof(SafeStep).Assembly);

        return services
            .AddLogging()
            .AddWorkflow(cfg =>
            {
                if (configureWorkflowEngine != null)
                {
                    configureWorkflowEngine(cfg);
                }
            })
            .AddSingleton<WorkflowHostedService>()
            .AddSingleton<IHostedService>(sp => sp.GetRequiredService<WorkflowHostedService>())
            .AddTransient<WorkflowService>();
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
