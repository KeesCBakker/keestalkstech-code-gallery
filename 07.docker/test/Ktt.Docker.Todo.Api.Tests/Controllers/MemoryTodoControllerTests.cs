using Ktt.Docker.Todo.Api.Tests.TestInfrastructure;

namespace Ktt.Docker.Todo.Api.Tests.Controllers;

[ClassDataSource<TestApplicationFactory>(Shared = SharedType.PerClass)]
[InheritsTests]
public class MemoryTodoControllerTests : TodoControllerTestsBase{
    public MemoryTodoControllerTests(TestApplicationFactory factory) : base(factory)
    {
    }
}
