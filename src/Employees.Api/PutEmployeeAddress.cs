namespace Employees.Api;

internal class PutEmployeeAddress
{
    public string? StreetName { get; set; }
    public string? HouseNumber { get; set; }
    public string? Town { get; set; }
    public string? Postcode { get; set; }
    public PutCoordinates? Coordinates { get; set; }
}
