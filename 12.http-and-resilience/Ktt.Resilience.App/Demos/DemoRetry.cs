using Ktt.Resilience.Clients.NSwag;

using ICustomHttpStatusApiClient = Ktt.Resilience.Clients.NSwag.HttpClients.HttpStatus.IHttpStatusApiClient;
using KiotaHttpStatusClient = Ktt.Resilience.Clients.Kiota.HttpClients.HttpStatus.HttpStatusClient;
using INSwagHttpStatusClient = Ktt.Resilience.Clients.NSwag.HttpClients.HttpStatus.IHttpStatusApiClient;

public class DemoRetry(
    ICustomHttpStatusApiClient customHttpStatusApiClient,
    KiotaHttpStatusClient kiotaHttpStatusClient,
    INSwagHttpStatusClient nSwagHttpStatusClient
)
{
    public async Task RunAsync()
    {
        await Execute("CustomHttpStatusApiClient", async () => await customHttpStatusApiClient.GetRandomStatusAsync());
        await Execute("KiotaHttpStatusClient", async () => await kiotaHttpStatusClient.Random.TwoZeroZeroFiveZeroZeroFiveZeroTwoFiveZeroThree.GetAsync());
        await Execute("NSwagHttpStatusClient", async () => await nSwagHttpStatusClient.GetRandomStatusAsync());

        Console.WriteLine("");
    }

    private async Task Execute(string name, Func<Task<string?>> execute)
    {
        Console.WriteLine($"Retry Demo for {name}...");

        for (var i = 0; i < 4; i++)
        {
            Console.WriteLine("");
            Console.WriteLine($"Calling {name} iteration {i}");

            try
            {
                var str = await execute();
                Console.WriteLine($"Result: {str}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
