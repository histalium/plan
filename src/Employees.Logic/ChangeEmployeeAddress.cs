namespace Employees.Logic;

public class ChangeEmployeeAddress
{
    public readonly IEmployeeEventStore employeeEventStore;

    public ChangeEmployeeAddress(IEmployeeEventStore employeeEventStore)
    {
        this.employeeEventStore = employeeEventStore;
    }

    public async Task<OneOf<None, ErrorMessage>> ExecuteAsync(EmployeeId id, Address address)
    {
        var addressChangedEvent = new EmployeeAddressChanged(
            address.StreetName,
            address.HouseNumber,
            address.Town,
            address.Postcode,
            address.Coordinates);
        return await employeeEventStore.AddEventAsync(id, 1, addressChangedEvent);
    }
}
