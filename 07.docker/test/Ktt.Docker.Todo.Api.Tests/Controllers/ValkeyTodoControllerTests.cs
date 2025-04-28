using Ktt.Docker.Todo.Api.Tests.TestInfrastructure;

namespace Ktt.Docker.Todo.Api.Tests.Controllers;

[Trait("Category", "Integration")]
public class ValkeyTodoControllerTests : TodoControllerTests, IClassFixture<IntegrationTestApplicationFactory>
{
    public ValkeyTodoControllerTests(IntegrationTestApplicationFactory factory):
        base(factory)
    {

    }
}
