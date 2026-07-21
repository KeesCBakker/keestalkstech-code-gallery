namespace Ktt.Validation.Api.Services;

public interface IDockerHubService
{
    Task<bool> Exists(string repo, CancellationToken cancellationToken);
}

public class DockerHubService : IDockerHubService
{
    public Task<bool> Exists(string repo, CancellationToken cancellationToken)
    {
        // Simulate checking Docker Hub for the existence of the repo
        return Task.FromResult(false);
    }
}
