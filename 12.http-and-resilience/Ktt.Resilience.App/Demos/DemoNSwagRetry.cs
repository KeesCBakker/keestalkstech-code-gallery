using Ktt.Resilience.NSwagClients.HttpClients.HttpStatus;

public class DemoNSwagRetry(HttpStatusApiClient httpStatusClient)
{
    public async Task RunAsync()
    {
        Console.WriteLine("Calling DemoNSwagRetry...");

        for(var i = 0; i < 4; i++)
        {
            Console.WriteLine("");
            Console.WriteLine("Calling HttpStatusClient iteration " + i);

            try
            {
                var str = await httpStatusClient.GetRandomStatusAsync();
                Console.WriteLine("Result:" + str);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        Console.WriteLine("");
    }
}
