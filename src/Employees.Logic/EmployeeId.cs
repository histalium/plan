using System;

namespace Employees.Logic;

public record EmployeeId
{
    private EmployeeId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; init; }

    public static EmployeeId NewEmployeeId()
        => new EmployeeId(Guid.NewGuid());
}