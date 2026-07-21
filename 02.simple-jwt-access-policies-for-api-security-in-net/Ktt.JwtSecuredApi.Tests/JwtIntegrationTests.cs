using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Ktt.JwtSecuredApi.Tests;

public class JwtIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public JwtIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    public static TheoryData<string, string, int> TokenEndpointExpectedStatusData => new()
    {
        { Token.Service1, "/api/orders", StatusCodes.Status200OK },
        { Token.Service2, "/api/orders", StatusCodes.Status200OK },
        { Token.Service1, "/api/users", StatusCodes.Status403Forbidden },
        { Token.Service2, "/api/users", StatusCodes.Status200OK },
    };

    [Fact]
    public async Task Get_Products_WithoutToken_Returns200()
    {
        var response = await _client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Orders_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/orders");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_Users_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(TokenEndpointExpectedStatusData))]
    public async Task Get_Endpoint_WithToken_ReturnsExpectedStatus(string token, string endpoint, int expectedStatus)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        Assert.Equal(expectedStatus, (int)response.StatusCode);
    }

    [Fact]
    public async Task Get_WhoAmI_WithService1Token_ReturnsUserInfo()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/whoami");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.Service1);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<WhoAmIResponse>();

        Assert.NotNull(body);
        Assert.Equal("tstusr", body.UserName);
        Assert.Equal("service-1", body.Issuer);
    }

    [Fact]
    public async Task Get_WhoAmI_WithService2Token_ReturnsUserInfo()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/whoami");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.Service2);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<WhoAmIResponse>();

        Assert.NotNull(body);
        Assert.Equal("tstusr", body.UserName);
        Assert.Equal("service-2", body.Issuer);
    }

    private record WhoAmIResponse(string? UserName, string? Issuer);
}

file static class Token
{
    public const string Service1 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.ewogICJpc3MiOiAic2VydmljZS0xIiwKICAiYXVkIjogIm91ci1zZXJ2aWNlIiwKICAidXNlcm5hbWUiOiAidHN0dXNyIiwKICAiaWF0IjogMTczMjk3MjMwNSwKICAiZXhwIjogMjUyMTM3MjMwNQp9.wWJh41loGBZKyDYBr-U9EJReEPsO6PA9z-EYE5rXO44e6XPjcsAMigoVcrR2w0T8-6is5ICJy2fukwOPDMLk9D2bs8k7TSVEuqzwh80tlBMPV5dRdkq3r1dg_KRZgkzG4ylLiK9hBoqvmL5HKE7oqo3AvHoUc1LOD5Y6BzeqasxVfOpIcjIa2nNXRLeRE7KfffWcbKXOm6HpYL2n_8j4pVbCePo1D8jtg55EQATcr1QQpvERzr9p-_PHqaC8woookSXqclTrwt-cQPj4RsvCQUXpKNzggXYytzHAaTlRAInlZP34tiDenb1Qz3wTtsqCXsh92BFZYABoJjIDGxcI5Q";
    public const string Service2 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.ewogICJpc3MiOiAic2VydmljZS0yIiwKICAiYXVkIjogIm91ci1zZXJ2aWNlIiwKICAidXNlcm5hbWUiOiAidHN0dXNyIiwKICAiaWF0IjogMTczMjk3MjM2NywKICAiZXhwIjogMjUyMTM3MjM2Nwp9.VGl-UElY0x7rLxIXlsYY6Cbd-0CbZIpzGQ1mgF2Ux-uBkyr4DYopFmJ37TUgcJ0xi-r5Q8UuKsCRWnm6DChpC8-189U49YXVu2cLdI5CTVdui2HvsUHvo9mSB7Rb1aPpMbQOFG-RZr6JfQXwBG5VJlk7CW1cF44JWvilVksZltm6zH_6Megt1Rbx7YXKDHV-gKXWawaevhGKBVRgGsPh1qF3GgqL6I_Tf-ZMt3_kTzkMGom6r7VZlO3Ze4Y8u1odVm1ZAHFjVwZy2UvNyPdQHW92COR7YKMJStVqKlCkQ6JDwgtnCMvPIu9tgr9WYtQaAwh6P3EbUuyp56C0lvNOPQ";
}
