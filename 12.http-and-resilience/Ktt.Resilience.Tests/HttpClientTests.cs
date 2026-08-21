using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ktt.Resilience.Clients.Kiota.HttpClients.PetStore;
using Ktt.Resilience.Clients.Kiota.HttpClients.PetStore.Models;
using Ktt.Resilience.Clients.Kiota.HttpClients.PetStore.Pet.FindByStatus;
using Ktt.Resilience.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Bundle;
using RichardSzalay.MockHttp;

namespace Ktt.Resilience.Tests;

public class HttpClientTests
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    [Test]
    public async Task KiotaPetStoreClientWithMockedObjects()
    {
        // arrange
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

        var jsonData = JsonSerializer.Serialize(pets, SerializerOptions);

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

        // act
        var result = await client.Pet.FindByStatus.GetAsync(x =>
        {
            x.QueryParameters.Status = [GetStatusQueryParameterType.Available];
        });

        // assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result).Count().IsEqualTo(2);

        await Assert.That(result[0].Id).IsEqualTo(42);
        await Assert.That(result[0].Name).IsEqualTo("Bandit Heeler");

        await Assert.That(result[1].Id).IsEqualTo(1337);
        await Assert.That(result[1].Name).IsEqualTo("Scooby-Doo");
    }

    [Test]
    public async Task KiotaPetStoreClientWithString()
    {
        // arrange
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

        // act
        var result = await client.Pet.FindByStatus.GetAsync(x =>
        {
            x.QueryParameters.Status = [GetStatusQueryParameterType.Available];
        });

        // assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result).Count().IsEqualTo(2);

        await Assert.That(result[0].Id).IsEqualTo(42);
        await Assert.That(result[0].Name).IsEqualTo("Bandit Heeler");

        await Assert.That(result[1].Id).IsEqualTo(1337);
        await Assert.That(result[1].Name).IsEqualTo("Scooby-Doo");
    }

    [Test]
    public async Task KiotaPetStoreMockedClient()
    {
        // arrange
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

        var mock = new MockedPetStoreClientFactory
        {
            Pets = [.. pets]
        };

        var client = mock.CreateClient();

        // act
        var availablePets = await client.Pet.FindByStatus.GetAsync(x =>
        {
            x.QueryParameters.Status = [GetStatusQueryParameterType.Available];
        });
        var pendingPets = await client.Pet.FindByStatus.GetAsync(x =>
        {
            x.QueryParameters.Status = [GetStatusQueryParameterType.Pending];
        });
        var soldPets = await client.Pet.FindByStatus.GetAsync(x =>
        {
            x.QueryParameters.Status = [GetStatusQueryParameterType.Sold];
        });

        // assert
        await Assert.That(availablePets).IsNotNull();
        await Assert.That(availablePets!).Count().IsEqualTo(1);
        await Assert.That(availablePets[0].Id).IsEqualTo(42);

        await Assert.That(pendingPets).IsNotNull();
        await Assert.That(pendingPets!).Count().IsEqualTo(1);
        await Assert.That(pendingPets[0].Id).IsEqualTo(1337);

        await Assert.That(soldPets).IsNotNull();
        await Assert.That(soldPets!).Count().IsEqualTo(1);
        await Assert.That(soldPets[0].Id).IsEqualTo(1950);
    }

    [Test]
    public async Task KiotaPetStoreDependencyInjection()
    {
        // arrange
        var factory = new MockedPetStoreClientFactory
        {
            Pets = {
                new Pet
                {
                    Id = 42,
                    Name = "Bandit Heeler",
                    Category = new Category { Id = 1, Name = "Dogs" },
                    PhotoUrls = [ "https://upload.wikimedia.org/wikipedia/en/9/90/Bandit_Heeler.png" ],
                    Tags = [ new Tag { Id = 1, Name = "cartoon" } ],
                    Status = Pet_status.Available
                }
            }
        };

        var services = new ServiceCollection();
        services.AddSingleton(x => factory.CreateClient());

        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<PetStoreClient>();

        // act
        var availablePets = await client.Pet.FindByStatus.GetAsync(x =>
        {
            x.QueryParameters.Status = [GetStatusQueryParameterType.Available];
        });

        // assert
        await Assert.That(availablePets).IsNotNull();
        await Assert.That(availablePets!).Count().IsEqualTo(1);
        await Assert.That(availablePets[0].Id).IsEqualTo(42);
    }
}
