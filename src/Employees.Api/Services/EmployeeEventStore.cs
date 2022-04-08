using System.Text.Json;
using Employees.Logic;
using Microsoft.Azure.Cosmos;
using OneOf;
using OneOf.Types;
using Plan.Utilities;

namespace Employees.Api;

public class EmployeeEventStore : IEmployeeEventStore
{
    private readonly CosmosClient cosmosClient;

    public EmployeeEventStore(CosmosClient cosmosClient)
    {
        this.cosmosClient = cosmosClient;
    }

    public async Task<OneOf<None, ErrorMessage>> AddEventAsync<T>(EmployeeId id, int expectedVersion,
        T employeeEvent)
    {
        try
        {
            switch (employeeEvent)
            {
                case EmployeeCreated created:
                    await AddToStoreAsync(nameof(EmployeeCreated), new EmployeeCreatedEventData(created));
                    break;

                case EmployeeNameChanged nameChanged:
                    await AddToStoreAsync(nameof(EmployeeNameChanged), new EmployeeNameChangedEventData(nameChanged));
                    break;

                case EmployeeAddressChanged addressChanged:
                    await AddToStoreAsync(nameof(EmployeeAddressChanged), new EmployeeAddressChangedEventData(addressChanged));
                    break;

                default:
                    return new ErrorMessage("Type not implemented");
            }

            return new None();
        }
        catch (Exception ex)
        {
            return new ErrorMessage(ex.Message);
        }

        async Task<Event<TEventData>> AddToStoreAsync<TEventData>(string eventName, TEventData eventData)
        {
            var lastVersion = await GetLastVersion(id);

            return await cosmosClient.GetContainer("ToDoList", "Items")
                .CreateItemAsync(new Event<TEventData>(id.Value, lastVersion + 1, eventName, eventData));
        }
    }

    public async Task<OneOf<IReadOnlyCollection<EmployeeEvent>, ErrorMessage>> GetEventsAsync(EmployeeId employeeId)
    {
        var events = cosmosClient.GetContainer("ToDoList", "Items")
            .GetItemQueryStreamIterator($"SELECT * FROM c WHERE c.Stream = '{employeeId.Value}' ORDER BY c.Version");

        using var e = await events.ReadNextAsync();
        var evs = await ParseEvents(e.Content).ToListAsync();

        var errs = evs
            .Where(t => t.IsT1)
            .Select(t => t.AsT1)
            .ToList();

        if (errs.Any())
        {
            return errs.First();
        }

        return evs
            .Where(t => t.IsT0)
            .Select(t => t.AsT0)
            .ToList();
    }

    private async IAsyncEnumerable<OneOf<EmployeeEvent, ErrorMessage>> ParseEvents(Stream stream)
    {
        using var document = await JsonDocument.ParseAsync(stream);
        JsonElement root = document.RootElement;
        JsonElement documentsElement = root.GetProperty("Documents");
        foreach (JsonElement d in documentsElement.EnumerateArray())
        {
            var name = d.GetProperty("Name");
            var data = d.GetProperty("Data");
            switch (name.GetString())
            {
                case nameof(EmployeeCreated):
                    {
                        var id = EmployeeId.FromGuid(data.GetProperty("Id").GetGuid());
                        var givenName = GivenName.ParseGivenName(data.GetProperty("GivenName").GetString());
                        var familyName = FamilyName.ParseFamilyName(data.GetProperty("FamilyName").GetString());

                        yield return givenName
                            .TupleOrError(familyName)
                            .Match<OneOf<EmployeeEvent, ErrorMessage>>(
                                ((GivenName GivenName, FamilyName FamilyName) t) => (EmployeeEvent)new EmployeeCreated(id, t.GivenName, t.FamilyName),
                                e => e);

                        break;
                    }

                case nameof(EmployeeNameChanged):
                    {
                        var givenName = GivenName.ParseGivenName(data.GetProperty("GivenName").GetString());
                        var familyName = FamilyName.ParseFamilyName(data.GetProperty("FamilyName").GetString());

                        yield return givenName
                            .TupleOrError(familyName)
                            .Match<OneOf<EmployeeEvent, ErrorMessage>>(
                                ((GivenName GivenName, FamilyName FamilyName) t) => (EmployeeEvent)new EmployeeNameChanged(t.GivenName, t.FamilyName),
                                e => e);

                        break;
                    }

                case nameof(EmployeeAddressChanged):
                    {
                        var streetName = StreetName.ParseStreetName(data.GetProperty("StreetName").GetString());
                        var houseNumber = HouseNumber.ParseHouseNumber(data.GetProperty("HouseNumber").GetString());
                        var town = Town.ParseTown(data.GetProperty("Town").GetString());
                        var postcode = Postcode.ParsePostcode(data.GetProperty("Postcode").GetString());
                        var coordinatesProperty = data.GetProperty("Coordinates");
                        var latitude = coordinatesProperty.GetProperty("Latitude").GetDouble();
                        var longitude = coordinatesProperty.GetProperty("Longitude").GetDouble();
                        var coordinates = Coordinates.ParseCoordinates(latitude, longitude);
                        yield return streetName
                            .TupleOrError(houseNumber)
                            .TupleOrError(town)
                            .TupleOrError(postcode)
                            .TupleOrError(coordinates)
                            .Match<OneOf<EmployeeEvent, ErrorMessage>>(
                                ((StreetName StreetName, HouseNumber HouseNumber, Town Town, Postcode Postcode, Coordinates Coordinates) t)
                                    => (EmployeeEvent)new EmployeeAddressChanged(t.StreetName, t.HouseNumber, t.Town, t.Postcode, t.Coordinates),
                                e => e);

                        break;
                    }

                default:
                    yield return new ErrorMessage($"Unknown event {name}");
                    break;
            }
        }
    }

    private async Task<int> GetLastVersion(EmployeeId employeeId)
    {
        var lastVersion = cosmosClient.GetContainer("ToDoList", "Items")
            .GetItemQueryIterator<LastStreamVersion>($"SELECT MAX(c.Version) LastVersion FROM c WHERE c.Stream = '{employeeId.Value}'");

        while (lastVersion.HasMoreResults)
        {
            var r = await lastVersion.ReadNextAsync();
            var s = r.FirstOrDefault();
            return s?.LastVersion ?? -1;
        }

        return -1;
    }
}

internal record ItemName(string Name);

internal record DocumentCollection<T>(T[] Documents);
