using Ktt.Workflows.Core.Steps;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps.Jenkins;

public class RunJenkinsJobStep : SafeStep
{
    public JenkinsJobDefinition Definition { get; set; } = default!;

    protected override ExecutionResult Execute(IStepExecutionContext context)
    {
        var d = Definition;

        var jenkinsUrl = $"https://jenkins.example.com/job/{d.JobName}";
        var output = $"Simulated Jenkins output for {d.JobName} with parameters: {string.Join(", ", d.Parameters)}";

        Data.SetStepState(context, new JenkinsStepResult
        {
            Url = jenkinsUrl,
            Output = output
        });

        Journal(context, $"Triggered Jenkins job: {d.JobName}");
        Journal(context, $"Jenkins URL: {jenkinsUrl}");
        Journal(context, $"Jenkins output:\n{output}");

        return ExecutionResult.Next();
    }

    public class JenkinsJobDefinition
    {
        public required string JobName { get; set; }

        public Dictionary<string, string> Parameters { get; set; } = new();
    }

    public class JenkinsStepResult
    {
        public required string Url { get; set; }

        public required string Output { get; set; } 
    }
}
