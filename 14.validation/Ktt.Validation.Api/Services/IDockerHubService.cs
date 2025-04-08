namespace Ktt.Validation.Api.Services;

public interface IDockerHubService
{
    Task<bool> Exists(string environment, string repo, CancellationToken cancellationToken);
}
