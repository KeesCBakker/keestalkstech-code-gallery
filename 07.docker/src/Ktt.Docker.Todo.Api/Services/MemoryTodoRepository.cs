using Ktt.Docker.Todo.Api.Models;
using System.Collections.Concurrent;

namespace Ktt.Docker.Todo.Api.Services;

public class MemoryTodoRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<string, TodoItem> _todos = new();

    public Task<IEnumerable<TodoItem>> ListAsync() =>
        Task.FromResult(_todos.Values.AsEnumerable());

    public Task<TodoItem?> GetAsync(string id) =>
        Task.FromResult(_todos.TryGetValue(id, out var item) ? item : null);

    public Task SaveAsync(TodoItem item)
    {
        _todos[item.Id] = item;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id)
    {
        _todos.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
