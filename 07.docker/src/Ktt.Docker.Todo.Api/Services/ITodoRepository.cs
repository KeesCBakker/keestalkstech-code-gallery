using Ktt.Docker.Todo.Api.Models;

namespace Ktt.Docker.Todo.Api.Services;

public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> ListAsync();

    Task<TodoItem?> GetAsync(string id);

    Task SaveAsync(TodoItem item);

    Task DeleteAsync(string id);
}
