using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ktt.Validation.Api.Config.Examples;

public class ParameterExamplesOperationFilter : IOperationFilter
{
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    if (operation.Parameters == null)
    {
      return;
    }

    foreach (var param in operation.Parameters)
    {
      if (param.Schema is not OpenApiSchema schema)
      {
        continue;
      }

      switch (param.Name)
      {
        case "team":
          schema.Example = JsonValue.Create(ProvisioningExamples.Team);
          break;
        case "environment":
          schema.Example = JsonValue.Create(ProvisioningExamples.Environment);
          break;
        case "applicationName":
          schema.Example = JsonValue.Create(ProvisioningExamples.ApplicationName);
          break;
      }
    }
  }
}
