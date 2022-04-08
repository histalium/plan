namespace Employees.Api;

internal class GetAddress
{
    public string StreetName { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public GetCoordinates Coordinates { get; set; } = new GetCoordinates();
}
