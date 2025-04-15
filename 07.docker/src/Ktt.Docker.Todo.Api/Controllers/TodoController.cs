using Ktt.Docker.Todo.Api.Models;
using Ktt.Docker.Todo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ktt.Docker.Todo.Api.Controllers;

[ApiController]
[Route("api/todos")]
public class TodoController : ControllerBase
{
    private readonly ITodoRepository _repository;

    public TodoController(ITodoRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IEnumerable<TodoItem>> List() =>
        await _repository.ListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> Get(string id)
    {
        var item = await _repository.GetAsync(id);
        return item is null ? NotFound() : item;
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create([FromBody] string title)
    {
        var item = new TodoItem(Guid.NewGuid().ToString(), title, false);
        await _repository.SaveAsync(item);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id}/text")]
    public async Task<IActionResult> UpdateText(string id, [FromBody] string newText)
    {
        var existing = await _repository.GetAsync(id);
        if (existing is null)
        {
            return NotFound();
        }

        var updated = existing with { Title = newText };
        await _repository.SaveAsync(updated);
        return NoContent();
    }

    [HttpPut("{id}/check")]
    public async Task<IActionResult> SetChecked(string id, [FromBody] bool completed)
    {
        var existing = await _repository.GetAsync(id);
        if (existing is null)
        {
            return NotFound();
        }

        var updated = existing with { Completed = completed };
        await _repository.SaveAsync(updated);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
