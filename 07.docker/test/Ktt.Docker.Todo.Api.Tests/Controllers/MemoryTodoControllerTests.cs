using Ktt.Docker.Todo.Api.Tests.TestInfrastructure;

namespace Ktt.Docker.Todo.Api.Tests.Controllers;

public class MemoryTodoControllerTests : TodoControllerTestsBase, IClassFixture<TestApplicationFactory>
{
    public MemoryTodoControllerTests(TestApplicationFactory factory) : base(factory)
    {
    }
}
