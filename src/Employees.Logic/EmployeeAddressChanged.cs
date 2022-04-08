namespace Employees.Logic;

public record EmployeeAddressChanged(
    StreetName StreetName,
    HouseNumber HouseNumber,
    Town Town,
    Postcode Postcode,
    Coordinates Coordinates);
