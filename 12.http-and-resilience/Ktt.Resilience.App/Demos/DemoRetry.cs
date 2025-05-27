using ICustomHttpStatusApiClient = Ktt.Resilience.Clients.HttpClients.HttpStatusApiService;
using KiotaHttpStatusClient = Ktt.Resilience.Clients.Kiota.HttpClients.HttpStatus.HttpStatusClient;

public class DemoRetry(
    ICustomHttpStatusApiClient customHttpStatusApiClient,
    KiotaHttpStatusClient kiotaHttpStatusClient
)
{
    public async Task RunAsync()
    {
        await Execute("CustomHttpStatusApiClient", async () => await customHttpStatusApiClient.Get());
        await Execute("KiotaHttpStatusClient", async () => await kiotaHttpStatusClient.Random.TwoZeroZeroFiveZeroZeroFiveZeroTwoFiveZeroThree.GetAsync());

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
