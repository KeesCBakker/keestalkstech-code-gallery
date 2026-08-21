using System.Net;
using System.Net.Http.Json;
using Ktt.Docker.Todo.Api.Models;
using Ktt.Docker.Todo.Api.Tests.TestInfrastructure;
using System.Threading.Tasks;

namespace Ktt.Docker.Todo.Api.Tests.Controllers;

public abstract class TodoControllerTestsBase(TestApplicationFactory factory)
{
    protected HttpClient Client { get; } = factory.CreateClient();

    [Test]
    public async Task Can_Create_Todo()
    {
        var response = await Client.PostAsJsonAsync("/api/todos", "Buy milk");
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<TodoItem>();
        await Assert.That(created).IsNotNull();
        await Assert.That(created!.Title).IsEqualTo("Buy milk");
        await Assert.That(created.Completed).IsFalse();
    }

    [Test]
    public async Task Can_Get_Todo()
    {
        var created = await CreateTodoAsync("Buy milk");

        var response = await Client.GetAsync($"/api/todos/{created.Id}");
        response.EnsureSuccessStatusCode();

        var fetched = await response.Content.ReadFromJsonAsync<TodoItem>();
        await Assert.That(fetched!.Id).IsEqualTo(created.Id);
        await Assert.That(fetched.Title).IsEqualTo(created.Title);
    }

    [Test]
    public async Task Can_Update_Text()
    {
        var created = await CreateTodoAsync("Buy milk");

        var newText = "Buy oat milk";
        var updateResponse = await Client.PutAsJsonAsync($"/api/todos/{created.Id}/text", newText);
        await Assert.That(updateResponse.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        var updated = await Client.GetFromJsonAsync<TodoItem>($"/api/todos/{created.Id}");
        await Assert.That(updated!.Title).IsEqualTo(newText);
    }

    [Test]
    public async Task Can_Check_And_Uncheck_Todo()
    {
        var created = await CreateTodoAsync("Buy milk");

        var checkResponse = await Client.PutAsJsonAsync($"/api/todos/{created.Id}/check", true);
        await Assert.That(checkResponse.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        var checkedItem = await Client.GetFromJsonAsync<TodoItem>($"/api/todos/{created.Id}");
        await Assert.That(checkedItem!.Completed).IsTrue();

        var uncheckResponse = await Client.PutAsJsonAsync($"/api/todos/{created.Id}/check", false);
        await Assert.That(uncheckResponse.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        var uncheckedItem = await Client.GetFromJsonAsync<TodoItem>($"/api/todos/{created.Id}");
        await Assert.That(uncheckedItem!.Completed).IsFalse();
    }

    [Test]
    public async Task Can_Delete_Todo()
    {
        var created = await CreateTodoAsync("Buy milk");

        var deleteResponse = await Client.DeleteAsync($"/api/todos/{created.Id}");
        await Assert.That(deleteResponse.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        var finalGet = await Client.GetAsync($"/api/todos/{created.Id}");
        await Assert.That(finalGet.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    private async Task<TodoItem> CreateTodoAsync(string title)
    {
        var response = await Client.PostAsJsonAsync("/api/todos", title);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TodoItem>())!;
    }
}