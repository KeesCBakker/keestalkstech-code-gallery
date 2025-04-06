namespace Ktt.Validation.Api;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args)
            .Build()
            .Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder =>
            {
                builder
                    .ConfigureKestrel(options =>
                    {
                        options.AddServerHeader = false;
                    })
                    .ConfigureAppConfiguration((builderContext, config) =>
                    {
                        config.AddJsonFile("appsettings.json", false, true);
                        config.AddEnvironmentVariables();
                    })
                    .UseStartup<Startup>();
            });
    }
}
