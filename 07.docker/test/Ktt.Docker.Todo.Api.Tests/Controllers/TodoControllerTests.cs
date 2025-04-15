using Ktt.Docker.Todo.Api.Models;
using Ktt.Todo.Api.Tests.TestInfrastructure;
using System.Net;
using System.Net.Http.Json;

namespace Ktt.Docker.Todo.Api.Tests.Controllers;

public abstract class TodoControllerTests(TestApplicationFactory factory)
{
    protected readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Can_Create_Todo()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", "Buy milk");
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(created);
        Assert.Equal("Buy milk", created!.Title);
        Assert.False(created.Completed);
    }

    [Fact]
    public async Task Can_Get_Todo()
    {
        var created = await CreateTodoAsync("Buy milk");

        var response = await _client.GetAsync($"/api/todos/{created.Id}");
        response.EnsureSuccessStatusCode();

        var fetched = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal(created.Title, fetched.Title);
    }

    [Fact]
    public async Task Can_Update_Text()
    {
        var created = await CreateTodoAsync("Buy milk");

        var newText = "Buy oat milk";
        var updateResponse = await _client.PutAsJsonAsync($"/api/todos/{created.Id}/text", newText);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var updated = await _client.GetFromJsonAsync<TodoItem>($"/api/todos/{created.Id}");
        Assert.Equal(newText, updated!.Title);
    }

    [Fact]
    public async Task Can_Check_And_Uncheck_Todo()
    {
        var created = await CreateTodoAsync("Buy milk");

        var checkResponse = await _client.PutAsJsonAsync($"/api/todos/{created.Id}/check", true);
        Assert.Equal(HttpStatusCode.NoContent, checkResponse.StatusCode);

        var checkedItem = await _client.GetFromJsonAsync<TodoItem>($"/api/todos/{created.Id}");
        Assert.True(checkedItem!.Completed);

        var uncheckResponse = await _client.PutAsJsonAsync($"/api/todos/{created.Id}/check", false);
        Assert.Equal(HttpStatusCode.NoContent, uncheckResponse.StatusCode);

        var uncheckedItem = await _client.GetFromJsonAsync<TodoItem>($"/api/todos/{created.Id}");
        Assert.False(uncheckedItem!.Completed);
    }

    [Fact]
    public async Task Can_Delete_Todo()
    {
        var created = await CreateTodoAsync("Buy milk");

        var deleteResponse = await _client.DeleteAsync($"/api/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var finalGet = await _client.GetAsync($"/api/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, finalGet.StatusCode);
    }

    private async Task<TodoItem> CreateTodoAsync(string title)
    {
        var response = await _client.PostAsJsonAsync("/api/todos", title);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TodoItem>())!;
    }
}
