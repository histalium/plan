namespace Employees.Logic;

public record Coordinates
{
    private Coordinates(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public double Latitude { get; init; }

    public double Longitude { get; init; }

    public static OneOf<Coordinates, ErrorMessage> ParseCoordinates(double? valueLatitude, double? valueLongitude)
    {
        if (!valueLatitude.HasValue)
        {
            return new ErrorMessage("Latitude can not be null.");
        }

        if (!valueLongitude.HasValue)
        {
            return new ErrorMessage("Longitude can not be null.");
        }

        if (valueLatitude < -90)
        {
            return new ErrorMessage("Latitude can not be below -90.");
        }

        if (valueLatitude > 90)
        {
            return new ErrorMessage("Latitude can not be above 90.");
        }

        if (valueLongitude < -180)
        {
            return new ErrorMessage("Longitude can not be below -180.");
        }

        if (valueLongitude > 180)
        {
            return new ErrorMessage("Longitude can not be above 180.");
        }

        return new Coordinates(valueLatitude.Value, valueLongitude.Value);
    }
}
