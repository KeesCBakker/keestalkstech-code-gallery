using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Services.Validation;

public class DataAnnotationsValidator(IServiceProvider provider) : IDataAnnotationsValidator
{
    protected virtual bool Validate(object obj, out IList<ValidationResult> validationErrors)
    {
        validationErrors = [];
        var context = new ValidationContext(obj, provider, null);
        var valid = Validator.TryValidateObject(obj, context, validationErrors, true);
        if (valid)
        {
            return true;
        }

        return false;
    }

    public bool TryValidate(object obj)
    {
        return Validate(obj, out _);
    }

    public bool TryValidate(object obj, out IList<ValidationResult> validationErrors)
    {
        return Validate(obj, out validationErrors);
    }

    public void ThrowIfInvalid(object obj, string? parameterName = null)
    {
        var valid = Validate(obj, out IList<ValidationResult> validationErrors);
        if (valid)
        {
            return;
        }

        var msg = GetErrorMessage(validationErrors, obj?.GetType());
        Exception ex = new ValidationException(msg);

        if (!string.IsNullOrEmpty(parameterName))
        {
            ex = new ArgumentException($"Parameter '{parameterName}' is invalid.", parameterName, ex);
        }

        throw ex;
    }

    public static string GetErrorMessage(IList<ValidationResult> validationErrors, Type? objectType)
    {
        ArgumentNullException.ThrowIfNull(validationErrors, nameof(validationErrors));

        var message = "Input invalid";
        if(objectType != null)
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
