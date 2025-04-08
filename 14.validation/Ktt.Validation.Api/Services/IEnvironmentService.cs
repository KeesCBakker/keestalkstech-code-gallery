namespace Ktt.Validation.Api.Services;

public interface IEnvironmentService
{
    Task<bool> Exists(string environment, CancellationToken cancellationToken);
}
