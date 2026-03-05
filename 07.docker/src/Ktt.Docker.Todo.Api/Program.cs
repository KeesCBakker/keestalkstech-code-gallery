using Ktt.Docker.Todo.Api.Services;
using Microsoft.OpenApi.Models;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddSingleton<ITodoRepository, MemoryTodoRepository>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Ktt Docker Todo API",
                Version = "v1"
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ktt Docker Todo API v1");
            options.RoutePrefix = "swagger";
        });

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
