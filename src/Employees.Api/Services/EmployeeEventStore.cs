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
                    await AddToStoreAsync(nameof(EmployeeCreated), new EmployeeNameChangedEventData(nameChanged));
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
