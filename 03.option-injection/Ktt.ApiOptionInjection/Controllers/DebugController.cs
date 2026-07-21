using Ktt.ApiOptionInjection.Config;
using Microsoft.AspNetCore.Mvc;

namespace Ktt.ApiOptionInjection.Controllers;

[ApiController]
[Route("api/debug")]
public class DebugController(
    SourceOptions sourceOptions,
    SupportedLanguagesOptions supportedLanguageOptions
) : ControllerBase
{
    [HttpGet]
    public DebugModel Get()
    {
        return new DebugModel
        {
            SourceOptions = sourceOptions,
            SupportedLanguagesOptions = supportedLanguageOptions
        };
    }
}

public class DebugModel
{
    public SourceOptions? SourceOptions { get; set; }

    public SupportedLanguagesOptions? SupportedLanguagesOptions { get; set; }
}
