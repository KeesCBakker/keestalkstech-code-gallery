using FluentAssertions;
using Ktt.Resilience.Clients.Kiota.HttpClients.PetStore;
using Ktt.Resilience.Clients.Kiota.HttpClients.PetStore.Models;
using Ktt.Resilience.Clients.Kiota.HttpClients.PetStore.Pet.FindByStatus;
using Ktt.Resilience.Tests.Mocks;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Bundle;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ktt.Resilience.Tests;

public class HttpClientTests
{
    [Fact]
    public async Task KiotaPetStoreClientWithMockedObjects()
    {
        var tag = new Tag { Id = 1, Name = "cartoon" };
        var category = new Category { Id = 1, Name = "Dogs" };
        var pets = new Pet[] {
            new Pet
            {
                Id = 42,
                Name = "Bandit Heeler",
                Category = category,
                PhotoUrls = [ "https://upload.wikimedia.org/wikipedia/en/9/90/Bandit_Heeler.png" ],
                Tags = [ tag ],
                Status = Pet_status.Available
            },
            new Pet
            {
                Id = 1337,
                Name = "Scooby-Doo",
                Category = category,
                PhotoUrls = [ "https://upload.wikimedia.org/wikipedia/en/5/53/Scooby-Doo.png" ],
                Tags = [ tag ],
                Status = Pet_status.Available
            }
        };

        var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        serializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        var jsonData = JsonSerializer.Serialize(pets, serializerOptions);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Get, "/v2/pet/findByStatus?status=available")
            .Respond(HttpStatusCode.OK, "application/json", jsonData);

        var client = new PetStoreClient(
            new DefaultRequestAdapter(
                new AnonymousAuthenticationProvider(),
                httpClient: mockHttp.ToHttpClient()
            )
        );

        var result = await client.Pet.FindByStatus.GetAsync(x =>
        {
            x.QueryParameters.Status = [GetStatusQueryParameterType.Available];
        });

        result.Should().NotBeNull();
        result.Count.Should().Be(2);

        result[0].Id.Should().Be(42);
        result[0].Name.Should().Be("Bandit Heeler");

        result[1].Id.Should().Be(1337);
        result[1].Name.Should().Be("Scooby-Doo");
    }

    [Fact]
    public async Task KiotaPetStoreClientWithString()
    {
        var json = @"
        [
          {
            ""id"": 42,
            ""category"": { ""id"": 1, ""name"": ""dog"" },
            ""name"": ""Bandit Heeler"",
            ""photoUrls"": [""https://upload.wikimedia.org/wikipedia/en/9/90/Bandit_Heeler.png""],
            ""tags"": [ { ""id"": 1, ""name"": ""cartoon"" } ],
            ""status"": ""available""
          },
          {
            ""id"": 1337,
            ""category"": { ""id"": 1, ""name"": ""dog"" },
            ""name"": ""Scooby-Doo"",
            ""photoUrls"": [""https://upload.wikimedia.org/wikipedia/en/5/53/Scooby-Doo.png""],
            ""tags"": [ { ""id"": 1, ""name"": ""cartoon"" } ],
            ""status"": ""available""
          }
        ]";

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Get, "/v2/pet/findByStatus?status=available")
            .Respond(HttpStatusCode.OK, "application/json", json);

        var client = new PetStoreClient(
            new DefaultRequestAdapter(
                new AnonymousAuthenticationProvider(),
                httpClient: mockHttp.ToHttpClient()
            )
        );

        var result = await client.Pet.FindByStatus.GetAsync(x =>
        {
            x.QueryParameters.Status = [GetStatusQueryParameterType.Available];
        });

        result.Should().NotBeNull();
        result.Count.Should().Be(2);

        result[0].Id.Should().Be(42);
        result[0].Name.Should().Be("Bandit Heeler");

        result[1].Id.Should().Be(1337);
        result[1].Name.Should().Be("Scooby-Doo");
    }

    [Fact]
    public async Task KiotaPetStoreMockedClient()
    {
        var tag = new Tag { Id = 1, Name = "cartoon" };
        var category = new Category { Id = 1, Name = "Dogs" };
        var pets = new Pet[] {
            new Pet
            {
                Id = 42,
                Name = "Bandit Heeler",
                Category = category,
                PhotoUrls = [ "https://upload.wikimedia.org/wikipedia/en/9/90/Bandit_Heeler.png" ],
                Tags = [ tag ],
                Status = Pet_status.Available
            },
            new Pet
            {
                Id = 1337,
                Name = "Scooby-Doo",
                Category = category,
                PhotoUrls = [ "https://upload.wikimedia.org/wikipedia/en/5/53/Scooby-Doo.png" ],
                Tags = [ tag ],
                Status = Pet_status.Pending
            },
            new Pet
            {
                Id = 1950,
                Name = "Snoopy",
                Category = category,
                PhotoUrls = [ "https://upload.wikimedia.org/wikipedia/en/5/53/Snoopy_Peanuts.png" ],
                Tags = [ tag ],
                Status = Pet_status.Sold
            }
        };

        var mock = new MockedPetStoreClient
        {
            Pets = pets.ToList()
        };

        var client = mock.CreateClient();

        var availablePets = await client.Pet.FindByStatus.GetAsync(x =>
        {
            x.QueryParameters.Status = [GetStatusQueryParameterType.Available];
        });
        availablePets.Should().HaveCount(1);
        availablePets[0].Id.Should().Be(42);

        var pendingPets = await client.Pet.FindByStatus.GetAsync(x =>
        {
            x.QueryParameters.Status = [GetStatusQueryParameterType.Pending];
        });
        pendingPets.Should().HaveCount(1);
        pendingPets[0].Id.Should().Be(1337);

        var soldPets = await client.Pet.FindByStatus.GetAsync(x =>
        {
            x.QueryParameters.Status = [GetStatusQueryParameterType.Sold];
        });
        soldPets.Should().HaveCount(1);
        soldPets[0].Id.Should().Be(1950);
    }
}
