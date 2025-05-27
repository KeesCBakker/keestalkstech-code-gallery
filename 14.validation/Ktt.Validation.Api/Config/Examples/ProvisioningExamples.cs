using Ktt.Validation.Api.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Ktt.Validation.Api.Config.Examples;

public class ProvisioningExamples
{
    public static string Name => "Example Application";

    public class SimpleApplicationExample : IExamplesProvider<SimpleApplication>
    {
        public SimpleApplication GetExamples() => new()
        {
            Name = Name,
            Label = "development",
            MagicNumber = 42,
            Type = ApplicationType.Application
        };
    }

    public class ComplexApplicationExample : IExamplesProvider<ComplexApplication>
    {
        public ComplexApplication GetExamples() => new()
        {
            Name = Name,
            Type = ComplexApplicationType.Application,
            DockerHubRepo = "ktt/example-application",
            Cpu = "250m",
            Ram = "128Mi",
            ImageTag = "latest",
            Environment = "server-one"
        };
    }
}
