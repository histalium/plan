namespace Employees.Logic;

public interface IEmployeeEventStore
{
    Task<OneOf<None, ErrorMessage>> AddEventAsync(EmployeeId id, int expectedVersion, EmployeeCreated employeeEvent);
}
