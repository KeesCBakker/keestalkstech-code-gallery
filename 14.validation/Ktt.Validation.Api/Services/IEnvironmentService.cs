namespace Ktt.Validation.Api.Services;

public interface IEnvironmentService
{
    Task<bool> Exists(string environment, CancellationToken cancellationToken);
}

public class EnvironmentService : IEnvironmentService
{
    public Task<bool> Exists(string environment, CancellationToken cancellationToken)
    {
        // Simulate checking Docker Hub for the existence of the repo
        return Task.FromResult(false);
    }
}
