using Ktt.Workflows.Core;
using System.Security.Cryptography;
using System.Text;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Ktt.Workflows.Implementation.Steps.Resources;

public class GeneratePasswordStep : SafeStep
{
    public int Length { get; set; } = 16;

    protected override Task<ExecutionResult> ExecuteAsync(IStepExecutionContext context)
    {
        var password = GenerateSecurePassword(Length);

        // Store in step state for later retrieval
        Data.SetFormValue(WorkflowFormKeys.GeneratedPassword, password);

        Journal(context, $"Generated a {Length}-character password.");
        return Next();
    }


    private static string GenerateSecurePassword(int length)
    {
        const string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+";
        var bytes = RandomNumberGenerator.GetBytes(length);
        var result = new StringBuilder(length);
        foreach (var b in bytes)
        {
            result.Append(charset[b % charset.Length]);
        }
        return result.ToString();
    }

    public class PasswordResult
    {
        public string Password { get; set; } = default!;
    }
}
