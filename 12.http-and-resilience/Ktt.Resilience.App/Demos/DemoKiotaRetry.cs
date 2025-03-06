﻿
using Ktt.Resilience.KiotaClients.HttpClients.HttpStatus;

public class DemoKiotaRetry(HttpStatusClient httpStatusClient)
{
    public async Task RunAsync()
    {
        Console.WriteLine("Calling DemoKiotaRetry...");

        for(var i = 0; i < 4; i++)
        {
            Console.WriteLine("");
            Console.WriteLine("Calling DemoKiotaRetry iteration " + i);

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
