using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/debug")]
public class DebugController(
    SourceOptions sourceOptions,
    SupportedLanguageOptions supportedLanguageOptions
) : ControllerBase
{
    [HttpGet]
    public DebugModel Get()
    {
        return new DebugModel
        {
            SourceOptions = sourceOptions,
            SupportedLanguageOptions = supportedLanguageOptions
        };
    }
}

public class DebugModel
{
    public SourceOptions? SourceOptions { get; set; }

    public SupportedLanguageOptions? SupportedLanguageOptions { get; set; }
}
