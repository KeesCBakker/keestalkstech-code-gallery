using System.ComponentModel.DataAnnotations;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Core.Steps;

public class ValidateWorkflowDataStep : SafeStep
{
    protected override Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(Data);

        if (!Validator.TryValidateObject(Data, validationContext, validationResults, true))
        {
            var errorMessages = string.Join("\n- ", validationResults.Select(v => v.ErrorMessage));
            throw new ValidationException($"Validation failed:\n- {errorMessages}");
        }

        var next = ExecutionResult.Next();
        return Task.FromResult(next);
    }
}
