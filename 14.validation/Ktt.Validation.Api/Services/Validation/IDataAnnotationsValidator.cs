using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Ktt.Validation.Api.Services.Validation;

public interface IDataAnnotationsValidator
{
    bool TryValidate(object obj);

    bool TryValidate(
        object obj,
        out IList<ValidationResult> validationErrors
    );

    void ThrowIfInvalid(
        object argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null
    );
}
