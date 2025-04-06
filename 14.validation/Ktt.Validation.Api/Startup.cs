using Ktt.Validation.Api.Services.Validation;
using Ktt.Validation.Api.Services;
using System.Text.Json.Serialization;
using Ktt.Validation.Api.Config;

namespace Ktt.Validation.Api;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IMagicNumberProvider, MagicNumberProvider>();
        services.AddTransient<IDataAnnotationsValidator, DataAnnotationsValidator>();
        services.AddTransient<ProvisionerService>();
        services.AddTransient((serviceProvider) => serviceProvider);

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
        app.UseResponseCompression();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
