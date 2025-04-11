using FluentValidation;
using FluentValidation.Resources;
using System.Globalization;

namespace Ktt.Validation.Api.Services.Validation;

public class FluentValidationLanguageManager : LanguageManager
{
    private FluentValidationLanguageManager()
    {
    }

    public override string? GetString(string key, CultureInfo? culture = null)
    {
        var message = base.GetString(key, culture);

        // Harmonize output with attribute validation
        return message?.Replace("'{PropertyName}'", "{PropertyName}");
    }

    public static void SetGlobalOptions()
    {
        ValidatorOptions.Global.LanguageManager = new FluentValidationLanguageManager();
        ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) =>
        {
            return member?.Name;
        };
    }
}
