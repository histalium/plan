namespace Employees.Logic;

public record Employee(
    EmployeeId Id,
    GivenName GivenName,
    FamilyName FamilyName,
    OneOf<Address, None> Address
);
