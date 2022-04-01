using Employees.Logic;

namespace Employees.Api;

public record EmployeeCreatedEventData
{
    public EmployeeCreatedEventData()
    {
        Id = Guid.Empty;
        GivenName = string.Empty;
        FamilyName = string.Empty;
    }

    public EmployeeCreatedEventData(EmployeeCreated employeeCreated)
    {
        Id = employeeCreated.Id.Value;
        GivenName = employeeCreated.GivenName.Value;
        FamilyName = employeeCreated.FamilyName.Value;
    }

    public Guid Id { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
}
