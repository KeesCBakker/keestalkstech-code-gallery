using Ktt.Docker.Todo.Api.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Ktt.Docker.Todo.Api.Services;

public class ValkeyTodoRepository : ITodoRepository
{
    private readonly IDatabase _db;
    private const string KeyPrefix = "todo:";

    public ValkeyTodoRepository(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<IEnumerable<TodoItem>> ListAsync()
    {
        var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{KeyPrefix}*").ToArray();
        var items = new List<TodoItem>();

        foreach (var key in keys)
        {
            var json = await _db.StringGetAsync(key);
            if (!json.IsNullOrEmpty)
            {
                items.Add(JsonSerializer.Deserialize<TodoItem>(json!)!);
            }
        }

        return items;
    }

    public async Task<TodoItem?> GetAsync(string id)
    {
        var json = await _db.StringGetAsync($"{KeyPrefix}{id}");
        return json.IsNullOrEmpty ? null : JsonSerializer.Deserialize<TodoItem>(json!);
    }

    public async Task SaveAsync(TodoItem item)
    {
        var json = JsonSerializer.Serialize(item);
        await _db.StringSetAsync($"{KeyPrefix}{item.Id}", json);
    }

    public Task DeleteAsync(string id) =>
        _db.KeyDeleteAsync($"{KeyPrefix}{id}");
}
