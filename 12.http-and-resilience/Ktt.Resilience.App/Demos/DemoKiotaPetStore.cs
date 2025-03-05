using Ktt.Ktt.KiotaClients.HttpClients.PetStore;
using Ktt.Ktt.KiotaClients.HttpClients.PetStore.Pet.FindByStatus;
using System.Text.RegularExpressions;

public class DemoKiotaPetStore(PetStoreClient petStoreclient)
{
    public async Task RunAsync()
    {
        Console.WriteLine("Calling the Petstore...");

        var pets = await petStoreclient.Pet.FindByStatus.GetAsync(x =>
        {
            x.QueryParameters.Status = [GetStatusQueryParameterType.Available];
        });

        var list = pets!
            .Select(x => x.Name)
            .Where(x => x != null && Regex.IsMatch(x, "^[a-zA-Z]{1,5}$"))
            .OrderBy(x => x)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach(var p in list)
        {
            Console.WriteLine("- " + p);
        }

        Console.WriteLine("Query returned " + pets!.Count + " results, of which " + list.Count + " have a valid name.");
        Console.WriteLine();
    }
}
