using Microsoft.OpenApi.Models;
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
            options.SupportNonNullableReferenceTypes();
        });
    }
}
