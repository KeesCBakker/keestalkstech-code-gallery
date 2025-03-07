using Ktt.Resilience.Clients.Kiota.HttpClients.PetStore.Pet.FindByStatus;
using Ktt.Resilience.Clients.NSwag.HttpClients.PetStore;
using System.Text.RegularExpressions;

using INSwagPetStoreClient = Ktt.Resilience.Clients.NSwag.HttpClients.PetStore.IPetStoreApiClient;
using KiotaPetStoreClient = Ktt.Resilience.Clients.Kiota.HttpClients.PetStore.PetStoreClient;

public partial class DemoPetStore(KiotaPetStoreClient kiotaPetStoreClient, INSwagPetStoreClient nSwagPetStoreClient)
{
    public async Task RunAsync()
    {
        // execute the Kiota Demo
        Console.WriteLine("Calling KiotaPetStoreClient...");

        var kiotaClientPets = await kiotaPetStoreClient.Pet.FindByStatus.GetAsync(x =>
        {
            x.QueryParameters.Status = [GetStatusQueryParameterType.Available];
        });

        WriteResults(kiotaClientPets!.Select(x => x.Name));

        // execute the NSwag Demo
        Console.WriteLine("Calling NSwagPetStoreClient...");

        var nswagPets = await nSwagPetStoreClient.PetFindByStatusAsync([Anonymous.Available]);
        WriteResults(kiotaClientPets!.Select(x => x.Name));
    }

    private void WriteResults(IEnumerable<string?> names)
    {
        var array = names.ToArray();

        var list = array
            .Where(x => x != null && Regex.IsMatch(x, "^[a-zA-Z]{1,5}$"))
            .OrderBy(x => x)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Console.Write("We have the following pets: ");
        Console.WriteLine(string.Join(", ", list));
        Console.WriteLine();
        Console.WriteLine("Query returned " + array.Length + " results, of which " + list.Count + " have a valid name.");
        Console.WriteLine();
    }
}
