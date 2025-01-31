using Markdig;
using Microsoft.OpenApi.Models;
using System.Reflection;

public static class SwaggerExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(op =>
         {
             op.EnableAnnotations();
             op.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
             {
                 Name = "Authorization",
                 Type = SecuritySchemeType.Http,
                 Scheme = "Bearer",
                 BearerFormat = "JWT",
                 In = ParameterLocation.Header,
                 Description = "Enter the token (without Bearer)"
             });

             op.AddSecurityRequirement(new OpenApiSecurityRequirement
             {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
             });

             // Embed and process README.md
             var assembly = Assembly.GetExecutingAssembly();
             var resourceName = "Ktt.JwtSecuredApi.README.md";

             using StreamReader reader = new(assembly.GetManifestResourceStream(resourceName)!);
             string markdownContent = reader.ReadToEnd();

             // Extract the first line as title and remaining content as description
             var lines = markdownContent.Split('\n', 2, StringSplitOptions.RemoveEmptyEntries);
             var title = lines.Length > 0 ? lines[0].Trim('#', ' ', '\r') : "API Documentation"; // Assume the first line is Markdown header
             var descriptionMarkdown = lines.Length > 1 ? lines[1] : string.Empty;

             // Convert remaining Markdown to HTML
             var descriptionHtml = Markdown.ToHtml(descriptionMarkdown);

             // Set Swagger Documentation Info
             op.SwaggerDoc("v1", new OpenApiInfo
             {
                 Title = title,
                 Version = "v1",
                 Description = descriptionHtml
             });
         });
    }
}
