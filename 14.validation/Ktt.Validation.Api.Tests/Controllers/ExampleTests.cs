using FluentAssertions;
using Provisioner.Api.UnitTests;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Ktt.Validation.Api.Tests.Controllers;

public class SwaggerExampleTest
{
    private static readonly TestWebApplicationFactory _fixture = new TestWebApplicationFactory();
    private static readonly HttpClient _client = _fixture.CreateClient();
    private static readonly Dictionary<string, (string Method, string Path, string Json)> _exampleMap = new();

    static SwaggerExampleTest()
    {
        var json = _client.GetStringAsync("/swagger/v1/swagger.json").Result;
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

                _exampleMap[key] = (method.ToUpperInvariant(), path, exampleJson);
            }
        }
    }

    public static TheoryData<string> GetValidationEndpoints() =>
        [.. _exampleMap.Keys
            .Where(k => k.EndsWith("/validate"))];

    public static TheoryData<string> GetProvisioningEndpoints() =>
        [.. _exampleMap.Keys
            .Where(k => k.StartsWith("POST ") && !k.EndsWith("/validate"))];

    [Theory]
    [MemberData(nameof(GetValidationEndpoints))]
    public async Task Validate(string name)
    {
        var response = await SendExampleRequest(name);
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [MemberData(nameof(GetProvisioningEndpoints))]
    public async Task Provision(string name)
    {
        var response = await SendExampleRequest(name);
        response.EnsureSuccessStatusCode();
    }

    private static async Task<HttpResponseMessage> SendExampleRequest(string name)
    {
        if (!_exampleMap.TryGetValue(name, out var data))
        {
            throw new InvalidOperationException($"No data found for test case '{name}'.");
        }

        var (method, path, json) = data;

        var request = new HttpRequestMessage
        {
            Method = new HttpMethod(method),
            RequestUri = new Uri(_client.BaseAddress!, path),
            Content = new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        };

        var response = await _client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed {method} {path}: {response.StatusCode}\n{content}");
        }

        return response;
    }
}
