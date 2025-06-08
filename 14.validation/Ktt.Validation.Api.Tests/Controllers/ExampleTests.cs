using FluentAssertions;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using Provisioner.Api.UnitTests;
using System.Net.Http.Headers;
using System.Text;

namespace Ktt.Validation.Api.Tests.Controllers;

public class SwaggerExampleTest
{
    private static readonly TestWebApplicationFactory _fixture = new TestWebApplicationFactory();
    private static readonly HttpClient _client = _fixture.CreateClient();
    private static readonly Dictionary<string, (string Method, string Path, string Json)> _exampleMap = new();

    static SwaggerExampleTest()
    {
        using var stream = _client.GetStreamAsync("/swagger/v1/swagger.json").Result;
        var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

        if (diagnostic.Errors.Any())
        {
            throw new Exception("Failed to parse OpenAPI: " + string.Join(", ", diagnostic.Errors.Select(e => e.Message)));
        }

        foreach (var (path, pathItem) in openApiDoc.Paths)
        {
            foreach (var (method, operation) in pathItem.Operations)
            {
                if (operation.RequestBody?.Content == null)
                {
                    continue;
                }

                if (!operation.RequestBody.Content.TryGetValue("application/json", out var jsonContent))
                {
                    continue;
                }

                var example =
                    jsonContent.Example ??
                    jsonContent.Examples?.FirstOrDefault().Value?.Value;

                if (example == null)
                {
                    continue;
                }

                string exampleJson;
                using (var sw = new StringWriter())
                {
                    var jsonWriter = new OpenApiJsonWriter(sw);
                    example.Write(jsonWriter, OpenApiSpecVersion.OpenApi3_0);
                    exampleJson = sw.ToString();
                }

                var key = $"{method.ToString().ToUpperInvariant()} {path}";

                _exampleMap[key] = (method.ToString().ToUpperInvariant(), path, exampleJson);
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
