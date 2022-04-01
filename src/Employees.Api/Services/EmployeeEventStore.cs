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

    public async Task<OneOf<None, ErrorMessage>> AddEventAsync(EmployeeId id, int expectedVersion, EmployeeCreated employeeEvent)
    {
        try
        {
            await cosmosClient.GetContainer("ToDoList", "Items")
                .CreateItemAsync(new Event<EmployeeCreatedEventData>(id.Value, expectedVersion + 1, nameof(EmployeeCreated), new EmployeeCreatedEventData(employeeEvent)));

            return new None();
        }
        catch (Exception ex)
        {
            return new ErrorMessage(ex.Message);
        }
    }
}
