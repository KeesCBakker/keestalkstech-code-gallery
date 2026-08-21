using Ktt.Docker.Todo.Api.Tests.TestInfrastructure;

namespace Ktt.Docker.Todo.Api.Tests.Controllers;
[Property("Category", "Integration")]

[ClassDataSource<IntegrationTestApplicationFactory>(Shared = SharedType.PerClass)]
[InheritsTests]
public class ValkeyTodoControllerTests : TodoControllerTestsBase{
    public ValkeyTodoControllerTests(IntegrationTestApplicationFactory factory) :
        base(factory)
    {

    }
}
