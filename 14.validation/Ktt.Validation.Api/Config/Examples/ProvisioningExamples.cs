using Ktt.Validation.Api.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Ktt.Validation.Api.Config.Examples;

public class ProvisioningExamples
{
    public static string ApplicationName => "Example Application";

    public static string Environment => "server-one";

    public static string Team => "racing-green";

    public static ComplexApplication ComplexApplication => new()
    {
        Name = ApplicationName,
        Type = ComplexApplicationType.Application,
        DockerHubRepo = "ktt/example-application",
        Cpu = "250m",
        Ram = "128Mi",
        ImageTag = "latest",
        Environment = Environment,
        Team = Team
    };

    public class SimpleApplicationExample : IExamplesProvider<SimpleApplication>
    {
        public SimpleApplication GetExamples() => new()
        {
            Name = ApplicationName,
            Label = "development",
            MagicNumber = 42,
            Type = ApplicationType.Application
        };
    }

    public class ComplexApplicationExample : IExamplesProvider<ComplexApplication>
    {
        public ComplexApplication GetExamples() => ComplexApplication;
    }

    public class ComplexApplicationsExample : IExamplesProvider<ComplexApplication[]>
    {
        public ComplexApplication[] GetExamples() => [ComplexApplication];
    }
}
