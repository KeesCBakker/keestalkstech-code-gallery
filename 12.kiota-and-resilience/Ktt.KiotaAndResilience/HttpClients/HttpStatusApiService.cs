public class HttpStatusApiService(HttpClient client)
{
    public async Task<string> Get()
    {
        var path = "/random/200,500-511";
        return await client.GetStringAsync(path);
    }
}
