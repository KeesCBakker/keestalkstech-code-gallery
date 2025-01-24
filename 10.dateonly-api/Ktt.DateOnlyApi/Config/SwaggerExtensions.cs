using Microsoft.OpenApi.Models;

public static class SwaggerExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(op =>
         {
             op.EnableAnnotations();
             op.SwaggerDoc("v1", new OpenApiInfo
             {
                 Version = "v1",
             });
         });
    }
}