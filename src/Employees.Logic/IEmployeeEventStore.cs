namespace Employees.Logic;

public interface IEmployeeEventStore
{
    Task<OneOf<None, ErrorMessage>> AddEventAsync<T>(EmployeeId id, int expectedVersion,
        T employeeEvent);
    Task<OneOf<IReadOnlyCollection<EmployeeEvent>, ErrorMessage>>
        GetEventsAsync(EmployeeId employeeId);
}
