using DotNet.Testcontainers.Builders;
using Ktt.Docker.Todo.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace Ktt.Docker.Todo.Api.Tests.TestInfrastructure;

public class IntegrationTestApplicationFactory : TestApplicationFactory, IAsyncLifetime
{
    private IContainer _valkeyContainer = null!;
    private IConnectionMultiplexer _redis = null!;

    public async Task InitializeAsync()
    {
        _valkeyContainer = new ContainerBuilder()
            .WithImage("valkey/valkey:latest")
            .WithName("valkey-test")
            .WithPortBinding(6379, assignRandomHostPort: true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
            .Build();

        await _valkeyContainer.StartAsync();

        var host = _valkeyContainer.Hostname;
        var port = _valkeyContainer.GetMappedPublicPort(6379);

        _redis = await ConnectionMultiplexer.ConnectAsync($"{host}:{port}");
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        RemoveService<ITodoRepository>(services);

        services.AddSingleton(_ => _redis);
        services.AddSingleton<ITodoRepository, ValkeyTodoRepository>();
    }

    public async override ValueTask DisposeAsync()
    {
        await _redis.CloseAsync();
        await _valkeyContainer.DisposeAsync();

        await base.DisposeAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await DisposeAsync();
    }
}
