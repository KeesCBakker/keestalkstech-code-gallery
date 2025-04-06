using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Services.Validation;

public interface IDataAnnotationsValidator
{
    bool TryValidate(object obj);

    bool TryValidate(object obj, out IList<ValidationResult> validationErrors);

    void ThrowIfInvalid(object obj, string? parameterName = null);
}
