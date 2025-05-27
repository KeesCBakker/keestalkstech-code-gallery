using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Ktt.Validation.Api.Config.Examples;

public class ValidationProblemDetailsExample : IExamplesProvider<ValidationProblemDetails>
{
    public ValidationProblemDetails GetExamples() => new(new Dictionary<string, string[]>
    {
        { "Field1", new[] { "Field1 must be empty." } },
        { "Field2", new[] { "Field2 must exist." } }
    })
    {
        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        Title = "One or more validation errors occurred.",
        Status = 400,
        Extensions = { ["traceId"] = "00-9ea61978914f872ec9d97f90e878b82e-abb28d99281b9f7a-00" }
    };
}
