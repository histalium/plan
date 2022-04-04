namespace Employees.Logic;

public class ChangeEmployeeName
{
    private readonly IEmployeeEventStore employeeEventStore;

    public ChangeEmployeeName(IEmployeeEventStore employeeEventStore)
    {
        this.employeeEventStore = employeeEventStore;
    }

    public async Task<OneOf<None, ErrorMessage>> ExecuteAsync(EmployeeId id, GivenName givenName, FamilyName familyName)
    {
        var nameChangedEvent = new EmployeeNameChanged(id, givenName, familyName);
        return await employeeEventStore.AddEventAsync(id, 1, nameChangedEvent);
    }
}
