using static Employees.Logic.EmployeeId;

namespace Employees.Logic;

public class CreateEmployee
{
    private readonly IEmployeeEventStore employeeEventStore;

    public CreateEmployee(IEmployeeEventStore employeeEventStore)
    {
        this.employeeEventStore = employeeEventStore;
    }

    public async Task<OneOf<None, ErrorMessage>> ExecuteAsync(GivenName givenName, FamilyName familyName)
    {
        var id = NewEmployeeId();
        var createdEvent = new EmployeeCreated(id, givenName, familyName);
        return await employeeEventStore.AddEventAsync(id, -1, createdEvent);
    }
}
