namespace Ktt.Validation.Api.Tests.Fixtures;

public class ValidationErrorResponse
{
    public Dictionary<string, string[]> Errors { get; set; } = new();

    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int Status { get; set; }

    public string TraceId { get; set; } = string.Empty;
}
