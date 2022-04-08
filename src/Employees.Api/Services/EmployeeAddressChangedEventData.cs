using Employees.Logic;

namespace Employees.Api;

public class EmployeeAddressChangedEventData
{
    public EmployeeAddressChangedEventData()
    {
        Address = new AddressEventData();
    }

    public EmployeeAddressChangedEventData(EmployeeAddressChanged employeeAddressChanged)
    {
        Address = new AddressEventData(employeeAddressChanged);
    }

    public AddressEventData Address { get; set; }
}
