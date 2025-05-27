using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
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
            switch (param.Name)
            {
                case "team":
                    param.Schema.Example = new OpenApiString(ProvisioningExamples.Team);
                    break;
                case "environment":
                    param.Schema.Example = new OpenApiString(ProvisioningExamples.Environment);
                    break;
                case "applicationName":
                    param.Schema.Example = new OpenApiString(ProvisioningExamples.ApplicationName);
                    break;
            }
        }
    }
}
