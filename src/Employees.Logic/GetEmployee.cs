namespace Employees.Logic;

public class GetEmployee
{
    private readonly IEmployeeEventStore employeeEventStore;

    public GetEmployee(IEmployeeEventStore employeeEventStore)
    {
        this.employeeEventStore = employeeEventStore;
    }

    public async Task<OneOf<Employee, None, ErrorMessage>> ExecuteAsync(EmployeeId employeeId)
    {
        var events = await employeeEventStore.GetEventsAsync(employeeId);

        var employee = events.Match(
            es => es.Aggregate((OneOf<Employee, None, ErrorMessage>)new None(), BuildEmployee),
            e => e
        );

        return employee;
    }

    private OneOf<Employee, None, ErrorMessage> BuildEmployee(OneOf<Employee, None, ErrorMessage> prev,
        OneOf<EmployeeCreated, EmployeeNameChanged> curr)
    {
        return curr.Match<OneOf<Employee, None, ErrorMessage>>(
            created => new Employee(created.Id, created.GivenName, created.FamilyName, new None()),
            changed => prev.Match<OneOf<Employee, None, ErrorMessage>>(
                e => e with
                {
                    GivenName = changed.GivenName,
                    FamilyName = changed.FamilyName
                },
                n => n,
                e => e
            )
        );
    }
}
