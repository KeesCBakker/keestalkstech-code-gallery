using FluentValidation;
using Ktt.Validation.Api.Config;
using Ktt.Validation.Api.Services;
using Ktt.Validation.Api.Services.Validation;
using System.Text.Json.Serialization;

namespace Ktt.Validation.Api;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        FluentValidationLanguageManager.SetGlobalOptions();

        services.AddTransient<IMagicNumberProvider, MagicNumberProvider>();
        services.AddTransient<IDataAnnotationsValidator, DataAnnotationsValidator>();
        services.AddTransient<ProvisionerService>();
        services.AddTransient((serviceProvider) => serviceProvider);
        services.AddTransient<IDockerHubService, DockerHubService>();
        services.AddTransient<IEnvironmentService, EnvironmentService>();

        services.AddSwagger();

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseDeveloperExceptionPage();
        app.UseResponseCompression();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
