using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Ktt.Validation.Api.Services.Validation;

public class DataAnnotationsValidator(IServiceProvider provider) : IDataAnnotationsValidator
{
    protected virtual bool Validate(object obj, out IList<ValidationResult> validationErrors)
    {
        validationErrors = [];
        var context = new ValidationContext(obj, provider, null);
        var valid = Validator.TryValidateObject(obj, context, validationErrors, true);
        return valid;
    }

    public bool TryValidate(object obj) =>
        Validate(obj, out _);

    public bool TryValidate(object obj, out IList<ValidationResult> validationErrors) =>
        Validate(obj, out validationErrors);

    public void ThrowIfInvalid(
        object argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        var valid = Validate(argument, out IList<ValidationResult> validationErrors);
        if (valid)
        {
            return;
        }

        var msg = GetErrorMessage(validationErrors, argument?.GetType());
        Exception ex = new ValidationException(msg);

        if (!string.IsNullOrEmpty(paramName))
        {
            ex = new ArgumentException($"Value is invalid.", paramName, ex);
        }

        throw ex;
    }

    public static string GetErrorMessage(IList<ValidationResult> validationErrors, Type? objectType)
    {
        ArgumentNullException.ThrowIfNull(validationErrors);

        var message = "Input invalid";
        if (objectType != null)
        {
            message += $" for '{objectType.Name}'";
        }

        message += ":\n";
        message += string.Join("\n",
            validationErrors.Select(r =>
                string.Join(", ", r.MemberNames) +
                ": " +
                r.ErrorMessage
            )
        );

        return message;
    }
}
