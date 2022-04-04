using Employees.Logic;

namespace Employees.Api;

public record EmployeeNameChangedEventData
{
    public EmployeeNameChangedEventData()
    {
        GivenName = string.Empty;
        FamilyName = string.Empty;
    }

    public EmployeeNameChangedEventData(EmployeeNameChanged employeeNameChanged)
    {
        GivenName = employeeNameChanged.GivenName.Value;
        FamilyName = employeeNameChanged.FamilyName.Value;
    }

    public string GivenName { get; set; }
    public string FamilyName { get; set; }
}
