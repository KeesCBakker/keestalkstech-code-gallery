using DotNet.Testcontainers.Builders;
using Ktt.Docker.Todo.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TUnit.Core.Interfaces;

using IContainer = DotNet.Testcontainers.Containers.IContainer;
using System.Threading.Tasks;

namespace Ktt.Docker.Todo.Api.Tests.TestInfrastructure;

public class IntegrationTestApplicationFactory : TestApplicationFactory, IAsyncInitializer, IAsyncDisposable
{
    private IContainer? _valkeyContainer;
    private IConnectionMultiplexer? _redis;

    public async Task InitializeAsync()
    {
        _valkeyContainer = new ContainerBuilder("valkey/valkey:latest")
            .WithName("valkey-test")
            .WithPortBinding(6379, assignRandomHostPort: true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(6379))
            .Build();

        await _valkeyContainer.StartAsync();

        var host = _valkeyContainer.Hostname;
        var port = _valkeyContainer.GetMappedPublicPort(6379);

        _redis = await ConnectionMultiplexer.ConnectAsync($"{host}:{port}");
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        RemoveService<ITodoRepository>(services);

        services.AddSingleton(_ => _redis!);
        services.AddSingleton<ITodoRepository, ValkeyTodoRepository>();
    }

    public override async ValueTask DisposeAsync()
    {
        try
        {
            if (_redis is not null)
            {
                await _redis.CloseAsync();
                _redis.Dispose();
            }
        }
        finally
        {
            if (_valkeyContainer is not null)
            {
                await _valkeyContainer.DisposeAsync();
            }

            await base.DisposeAsync();
        }
    }

}
