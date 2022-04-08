namespace Employees.Logic;

public class EmployeeEvent : OneOfBase<EmployeeCreated, EmployeeNameChanged, EmployeeAddressChanged>
{
    public EmployeeEvent(EmployeeCreated created) : base(created) { }

    public EmployeeEvent(EmployeeNameChanged nameChanged) : base(nameChanged) { }

    public EmployeeEvent(EmployeeAddressChanged addressChanged) : base(addressChanged) { }

    public static implicit operator EmployeeEvent(EmployeeCreated value)
        => new EmployeeEvent(value);

    public static implicit operator EmployeeEvent(EmployeeNameChanged value)
        => new EmployeeEvent(value);

    public static implicit operator EmployeeEvent(EmployeeAddressChanged value)
        => new EmployeeEvent(value);
}
