using Employees.Logic;

namespace Employees.Api;

public class CoordinatesEventData
{
    public CoordinatesEventData()
    {

    }

    public CoordinatesEventData(Coordinates coordinates)
    {
        Latitude = coordinates.Latitude;
        Longitude = coordinates.Longitude;
    }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
