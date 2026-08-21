using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Ktt.Validation.Api.Tests.Fixtures;

namespace Ktt.Validation.Api.Tests.Controllers;

[NotInParallel]
public class SwaggerExampleTest
{
    private static readonly Dictionary<string, (string Method, string Path, string Json)> _exampleMap = LoadExamples();

    private static Dictionary<string, (string Method, string Path, string Json)> LoadExamples()
    {
        using var fixture = new TestWebApplicationFactory();
        using var client = fixture.CreateClient();
        var exampleMap = new Dictionary<string, (string Method, string Path, string Json)>();
        var json = client.GetStringAsync("/swagger/v1/swagger.json").GetAwaiter().GetResult();
        var doc = JsonNode.Parse(json)!;

        foreach (var (path, pathItem) in doc["paths"]!.AsObject())
        {
            if (pathItem is not JsonObject pathObj)
            {
                continue;
            }

            foreach (var (method, operation) in pathObj)
            {
                var requestBody = operation?["requestBody"];
                var content = requestBody?["content"];
                var jsonContent = content?["application/json"];

                var example =
                    jsonContent?["example"] ??
                    jsonContent?["examples"]?.AsObject().FirstOrDefault().Value?["value"];

                if (example == null)
                {
                    continue;
                }

                var exampleJson = example.ToJsonString(JsonSerializerOptions.Default);
                var key = $"{method.ToUpperInvariant()} {path}";

                exampleMap[key] = (method.ToUpperInvariant(), path, exampleJson);
            }
        }

        return exampleMap;
    }

    public static IEnumerable<Func<string>> GetValidationEndpoints() =>
        _exampleMap.Keys
            .Where(k => k.EndsWith("/validate"))
            .Select(name => new Func<string>(() => name));

    public static IEnumerable<Func<string>> GetProvisioningEndpoints() =>
        _exampleMap.Keys
            .Where(k => k.StartsWith("POST ") && !k.EndsWith("/validate"))
            .Select(name => new Func<string>(() => name));

    [Test]
    [MethodDataSource(nameof(GetValidationEndpoints))]
    public async Task Validate(string name)
    {
        await SendExampleRequest(name);
    }

    [Test]
    [MethodDataSource(nameof(GetProvisioningEndpoints))]
    public async Task Provision(string name)
    {
        await SendExampleRequest(name);
    }

    private static async Task SendExampleRequest(string name)
    {
        if (!_exampleMap.TryGetValue(name, out var data))
        {
            throw new InvalidOperationException($"No data found for test case '{name}'.");
        }

        var (method, path, json) = data;

        using var fixture = new TestWebApplicationFactory();
        using var client = fixture.CreateClient();
        using var request = new HttpRequestMessage
        {
            Method = new HttpMethod(method),
            RequestUri = new Uri(client.BaseAddress!, path),
            Content = new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        };

        using var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed {method} {path}: {response.StatusCode}\n{content}");
        }

        response.EnsureSuccessStatusCode();
    }
}
