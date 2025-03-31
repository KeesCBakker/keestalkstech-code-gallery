using Ktt.Resilience.Clients.Kiota.HttpClients.PetStore;
using Ktt.Resilience.Clients.Kiota.HttpClients.PetStore.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Bundle;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ktt.Resilience.Tests.Mocks;

public class MockedPetStoreClient
{
    private readonly JsonSerializerOptions _serializerOptions;

    public List<Pet> Pets { get; set; } = [];

    public MockedPetStoreClient()
    {
        _serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        _serializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    public PetStoreClient CreateClient()
    {
        string SerializePets(Pet_status status)
        {
            var pets = Pets.Where(x => x.Status == status);
            return JsonSerializer.Serialize(pets, _serializerOptions);
        }

        var mockHttp = new MockHttpMessageHandler();

        mockHttp
            .When(HttpMethod.Get, "/v2/pet/findByStatus?status=available")
            .Respond(HttpStatusCode.OK, "application/json", SerializePets(Pet_status.Available));

        mockHttp
            .When(HttpMethod.Get, "/v2/pet/findByStatus?status=pending")
            .Respond(HttpStatusCode.OK, "application/json", SerializePets(Pet_status.Pending));

        mockHttp
            .When(HttpMethod.Get, "/v2/pet/findByStatus?status=sold")
            .Respond(HttpStatusCode.OK, "application/json", SerializePets(Pet_status.Sold));

        var client = new PetStoreClient(
            new DefaultRequestAdapter(
                new AnonymousAuthenticationProvider(),
                httpClient: mockHttp.ToHttpClient()
            )
        );

        return client;

    }
}
