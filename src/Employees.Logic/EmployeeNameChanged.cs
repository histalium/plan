namespace Employees.Logic;

public record EmployeeNameChanged(
    EmployeeId Id,
    GivenName GivenName,
    FamilyName FamilyName);
