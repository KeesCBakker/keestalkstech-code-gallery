public class DemoRetry(HttpStatusApiService httpStatusApiService)
{
    public async Task RunAsync()
    {
        Console.WriteLine("Calling HttpStatusApiService...");

        for(var i = 0; i < 10; i++)
        {
            Console.WriteLine("");
            Console.WriteLine("Calling HttpStatusApiService iteration " + i);

            try
            {
                var str = await httpStatusApiService.Get();
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
