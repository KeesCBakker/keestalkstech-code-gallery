using FluentValidation;
using Ktt.Validation.Api.Services;
using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Models;

public class ApplicationProvisioningRequest : IValidatableObject
{
    [Required, MinLength(5)]
    public string Name { get; set; } = string.Empty;

    public ApplicationType Type { get; set; }

    public string? EntryPoint { get; set; }

    public int MagicNumber { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var v = new InlineValidator<ApplicationProvisioningRequest>();

        // conditional validation
        v.RuleFor(x => x.EntryPoint)
            .NotEmpty()
            .When(x => x.Type == ApplicationType.ApplicationWithEntryPoint)
            .Empty()
            .When(x => x.Type == ApplicationType.Application);

        // dependency injection
        var provider = validationContext.GetRequiredService<IMagicNumberProvider>();
        v.RuleFor(x => x.MagicNumber)
            .MustAsync(async (magicNumber, _) => magicNumber == await provider.GetMagicNumber())
            .WithMessage("Magic number is invalid.");

        var result = v.ValidateAsync(this).Result;
        var errors = result.Errors.Select(e => new ValidationResult(e.ErrorMessage, [e.PropertyName]));
        return errors;
    }
}
