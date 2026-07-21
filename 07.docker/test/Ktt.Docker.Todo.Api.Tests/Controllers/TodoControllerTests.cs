using System.Net;
using System.Net.Http.Json;
using Ktt.Docker.Todo.Api.Models;

namespace Ktt.Docker.Todo.Api.Tests.Controllers;

public abstract class TodoControllerTestsBase(TestApplicationFactory factory)
{
  protected HttpClient Client { get; } = factory.CreateClient();

  [Fact]
  public async Task Can_Create_Todo()
  {
    var response = await Client.PostAsJsonAsync("/api/todos", "Buy milk");
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

    var response = await Client.GetAsync($"/api/todos/{created.Id}");
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
    var updateResponse = await Client.PutAsJsonAsync($"/api/todos/{created.Id}/text", newText);
    Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

    var updated = await Client.GetFromJsonAsync<TodoItem>($"/api/todos/{created.Id}");
    Assert.Equal(newText, updated!.Title);
  }

  [Fact]
  public async Task Can_Check_And_Uncheck_Todo()
  {
    var created = await CreateTodoAsync("Buy milk");

    var checkResponse = await Client.PutAsJsonAsync($"/api/todos/{created.Id}/check", true);
    Assert.Equal(HttpStatusCode.NoContent, checkResponse.StatusCode);

    var checkedItem = await Client.GetFromJsonAsync<TodoItem>($"/api/todos/{created.Id}");
    Assert.True(checkedItem!.Completed);

    var uncheckResponse = await Client.PutAsJsonAsync($"/api/todos/{created.Id}/check", false);
    Assert.Equal(HttpStatusCode.NoContent, uncheckResponse.StatusCode);

    var uncheckedItem = await Client.GetFromJsonAsync<TodoItem>($"/api/todos/{created.Id}");
    Assert.False(uncheckedItem!.Completed);
  }

  [Fact]
  public async Task Can_Delete_Todo()
  {
    var created = await CreateTodoAsync("Buy milk");

    var deleteResponse = await Client.DeleteAsync($"/api/todos/{created.Id}");
    Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

    var finalGet = await Client.GetAsync($"/api/todos/{created.Id}");
    Assert.Equal(HttpStatusCode.NotFound, finalGet.StatusCode);
  }

  private async Task<TodoItem> CreateTodoAsync(string title)
  {
    var response = await Client.PostAsJsonAsync("/api/todos", title);
    response.EnsureSuccessStatusCode();
    return (await response.Content.ReadFromJsonAsync<TodoItem>())!;
  }
}
