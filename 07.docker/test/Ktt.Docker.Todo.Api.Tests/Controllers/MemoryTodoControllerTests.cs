using Ktt.Docker.Todo.Api.Tests.TestInfrastructure;

namespace Ktt.Docker.Todo.Api.Tests.Controllers;

public class MemoryTodoControllerTests : TodoControllerTests, IClassFixture<TestApplicationFactory>
{
    public MemoryTodoControllerTests(TestApplicationFactory factory) : base(factory)
    {
    }
}
