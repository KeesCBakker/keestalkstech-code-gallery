using Ktt.Validation.Api.Config.Examples;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace Ktt.Validation.Api.Config;

public static class SwaggerConfig
{
    private static readonly string ApiName = "Platform Provisioning";

    private static string XmlCommentsFilePath
    {
        get
        {
            var basePath = AppContext.BaseDirectory;
            var fileName = typeof(SwaggerConfig).GetTypeInfo().Assembly.GetName().Name + ".xml";
            return Path.Combine(basePath, fileName);
        }
    }

    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = ApiName,
                Version = "v1",
            });

            // Add XML comments for documentation
            options.IncludeXmlComments(XmlCommentsFilePath);

            // Non nullable types render as required
            options.SupportNonNullableReferenceTypes();

            // Automatically discover examples
            options.ExampleFilters();

            // Add examples for parameters
            options.OperationFilter<ParameterExamplesOperationFilter>();

            // Turn config values into enums in the schema
            options.SchemaFilter<ConfigValuesSchemaFilter>();

        });

        services.AddSwaggerExamplesFromAssemblyOf<Startup>();
    }
}
