using Employees.Logic;

namespace Employees.Api;

public class EmployeeAddressChangedEventData
{

    public EmployeeAddressChangedEventData()
    {
        StreetName = string.Empty;
        HouseNumber = string.Empty;
        Town = string.Empty;
        Postcode = string.Empty;
        Coordinates = new CoordinatesEventData();
    }

    public EmployeeAddressChangedEventData(EmployeeAddressChanged address)
    {
        StreetName = address.StreetName.Value;
        HouseNumber = address.HouseNumber.Value;
        Town = address.Town.Value;
        Postcode = address.Postcode.Value;
        Coordinates = new CoordinatesEventData(address.Coordinates);
    }

    public string StreetName { get; set; }
    public string HouseNumber { get; set; }
    public string Town { get; set; }
    public string Postcode { get; set; }
    public CoordinatesEventData Coordinates { get; set; }
}
