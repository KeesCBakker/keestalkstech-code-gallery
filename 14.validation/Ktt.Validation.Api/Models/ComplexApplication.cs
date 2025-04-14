using FluentValidation;
using Ktt.Validation.Api.Services;
using NCrontab;
using System.ComponentModel.DataAnnotations;

namespace Ktt.Validation.Api.Models;

public class ComplexApplication : IValidatableObject
{
    [Required]
    public string Environment { get; set; } = string.Empty;

    public ComplexApplicationType Type { get; set; }

    [Required, MinLength(3), MaxLength(75)]
    public string DockerHubRepo { get; set; } = string.Empty;

    [Required]
    public string ImageTag { get; set; } = string.Empty;

    [Required, RegularExpression("^\\d+m$")]
    public string Cpu { get; set; } = string.Empty;

    [Required, RegularExpression("^\\d+Mi$")]
    public string Ram { get; set; } = string.Empty;

    public string Command { get; set; } = string.Empty;

    public string Schedule { get; set; } = string.Empty;

    [RegularExpression(@"^$|^(?!.*(cron|site|service))([a-z0-9-]*)$", ErrorMessage = "The value must be lower-kebab-case and may not contain the words cron, site or service.")]
    public string Postfix { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var dockerHubService = validationContext.GetRequiredService<IDockerHubService>();
        var environmentService = validationContext.GetRequiredService<IEnvironmentService>();

        var v = new InlineValidator<ComplexApplication>();

        v.RuleFor(x => x.DockerHubRepo)
            .NotEmpty()
            .MustAsync(dockerHubService.Exists)
            .WithMessage("The DockerHub repository does not exist.");

        v.RuleFor(x => x.Environment)
            .NotEmpty()
            .MustAsync(environmentService.Exists)
            .WithMessage("Environment must exist.");

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

