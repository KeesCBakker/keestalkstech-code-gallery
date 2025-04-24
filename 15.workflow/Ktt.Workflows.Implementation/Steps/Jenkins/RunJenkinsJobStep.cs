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

        context.SetStepState("jenkinsUrl", jenkinsUrl);
        context.SetStepState("jenkinsOutput", output);

        Journal(context, $"Jenkins job '{d.JobName}' triggered.");
        Journal(context, $"URL: {jenkinsUrl}");
        Journal(context, $"Output:\n{output}");

        return ExecutionResult.Next();
    }

    public class JenkinsJobDefinition
    {
        public string JobName { get; set; } = default!;
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}
