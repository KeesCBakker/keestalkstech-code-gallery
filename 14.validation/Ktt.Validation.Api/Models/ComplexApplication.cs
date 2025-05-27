using FluentValidation;
using Ktt.Validation.Api.Services;
using Ktt.Validation.Api.Services.Validation.Attributes;
using NCrontab;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ktt.Validation.Api.Models;

public class ComplexApplication : IValidatableObject
{
    [Required]
    public string Name { get; set; } = default!;

    [Required, Team]
    public string Team { get; set; } = default!;

    [Required, Environment]
    public string Environment { get; set; } = default!;

    public ComplexApplicationType Type { get; set; }

    [Required, MinLength(3), MaxLength(75)]
    public string DockerHubRepo { get; set; } = default!;

    [Required]
    public string ImageTag { get; set; } = default!;

    [Required, RegularExpression("^\\d+m$")]
    public string Cpu { get; set; } = default!;

    [Required, RegularExpression("^\\d+Mi$")]
    public string Ram { get; set; } = default!;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Command { get; set; } = default!;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Schedule { get; set; } = default!;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [RegularExpression(@"^$|^(?!.*(cron|site|service))([a-z0-9-]*)$", ErrorMessage = "The value must be lower-kebab-case and may not contain the words cron, site or service.")]
    public string? Postfix { get; set; } = default!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var dockerHubService = validationContext.GetRequiredService<IDockerHubService>();

        var v = new InlineValidator<ComplexApplication>();

        v.RuleFor(x => x.DockerHubRepo)
            .NotEmpty()
            .MustAsync(dockerHubService.Exists)
            .WithMessage("The DockerHub repository does not exist.");

        var isTypeWithCommand = Type is ComplexApplicationType.ApplicationWithCommand or ComplexApplicationType.CronJobWithCommand;
        if (isTypeWithCommand)
        {
            v.RuleFor(x => x.Command).NotEmpty();
            v.RuleFor(x => x.Command)
                .Must(cmd => string.IsNullOrEmpty(cmd) || !cmd.Contains(".sh") || cmd.Contains("tini"))
                .WithMessage("Script files (.sh) may only be executed when tini is used.");

            v.RuleFor(x => Postfix)
                .NotEmpty();
        }
        else
        {
            v.RuleFor(x => x.Command).Empty();
            v.RuleFor(x => x.Postfix).Empty();
        }

        var isCronJobType = Type is ComplexApplicationType.CronJob or ComplexApplicationType.CronJobWithCommand;
        if (isCronJobType)
        {
            v.RuleFor(x => x.Schedule).NotEmpty();
            v.RuleFor(x => x.Schedule)
                .Must(x => !string.IsNullOrEmpty(x) && CrontabSchedule.TryParse(x) != null)
                .WithMessage("Schedule must be a valid cron expression.");
        }
        else
        {
            v.RuleFor(x => x.Schedule).Empty();
        }

        var result = v.ValidateAsync(this).Result;
        var errors = result.Errors.Select(e => new ValidationResult(e.ErrorMessage, [e.PropertyName]));
        return errors;
    }
}

public enum ComplexApplicationType
{
    Application = 0,
    ApplicationWithCommand = 1,
    CronJob = 2,
    CronJobWithCommand = 3
}

