using FluentValidation;
using Ktt.Validation.Api.Services;
using Ktt.Validation.Api.Services.Validation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Models;

public class ApplicationProvisioningRequest : IValidatableObject
{
    [Required, MinLength(5), ApplicationNameAvailable]
    public string Name { get; set; } = string.Empty;

    public ApplicationType Type { get; set; }

    public string? EntryPoint { get; set; }

    public int MagicNumber { get; set; }

    [ValidLabel]
    public string Label { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var v = new InlineValidator<ApplicationProvisioningRequest>();

        // conditional validation
        v.RuleFor(x => x.EntryPoint)
            .NotEmpty()
            .When(x => x.Type == ApplicationType.ApplicationWithEntryPoint, ApplyConditionTo.CurrentValidator)
            .Empty()
            .When(x => x.Type == ApplicationType.Application, ApplyConditionTo.CurrentValidator);

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
