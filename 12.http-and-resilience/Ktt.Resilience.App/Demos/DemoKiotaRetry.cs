using Ktt.Ktt.KiotaClients.HttpClients.HttpStatus;

public class DemoKiotaRetry(HttpStatusClient httpStatusClient)
{
    public async Task RunAsync()
    {
        Console.WriteLine("Calling HttpStatusClient...");

        for(var i = 0; i < 4; i++)
        {
            Console.WriteLine("");
            Console.WriteLine("Calling HttpStatusClient iteration " + i);

            try
            {
                var str = await httpStatusClient.Random.TwoZeroZeroFiveZeroZeroFiveZeroTwoFiveZeroThree.GetAsync();
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
