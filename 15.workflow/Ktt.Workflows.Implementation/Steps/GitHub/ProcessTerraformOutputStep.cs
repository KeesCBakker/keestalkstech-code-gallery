using Ktt.Workflows.Core.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

public class ProcessTerraformOutputStep : SafeStep
{
    public TerraformOutputProcessor? Process { get; set; }

    protected override Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
    {
        var output = "test";

        if (Process != null)
        {
            Process(output, Data, context);
        }

        Journal(context, "Processed Terraform output");

        return Next();
    }
}

public delegate void TerraformOutputProcessor(string output, IWorkflowDataWithState data, IStepExecutionContext context);
