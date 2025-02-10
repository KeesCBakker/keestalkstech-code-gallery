public class HttpStatusApiService(HttpClient client)
{
    public async Task<string> Get()
    {
        var path = "/random/200,500-508";
        return await client.GetStringAsync(path);
    }
}
