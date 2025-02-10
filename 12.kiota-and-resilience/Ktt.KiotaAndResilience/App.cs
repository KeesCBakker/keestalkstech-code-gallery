public class App(HttpStatusApiService httpStatusApiService)
{
    public async Task RunAsync()
    {
        await CallHttpStatusApi();

        Console.WriteLine("Finished");
    }

    private async Task CallHttpStatusApi()
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
    }
}
