namespace Employees.Logic;

public record Address(
    StreetName StreetName,
    HouseNumber HouseNumber,
    Town Town,
    Postcode Postcode,
    Coordinates Coordinates)
{
    public static OneOf<Address, ErrorMessage> ParseAddress(string valueStreetName,
        string valueHouseNumber, string valueTown, string valuePostcode,
        double valueLatitude, double valueLongitude)
    {
        var streetName = StreetName.ParseStreetName(valueStreetName);
        var houseNumber = HouseNumber.ParseHouseNumber(valueHouseNumber);
        var town = Town.ParseTown(valueTown);
        var postcode = Postcode.ParsePostcode(valuePostcode);
        var coordinates = Coordinates.ParseCoordinates(valueLatitude, valueLongitude);

        var address = streetName
            .TupleOrError(houseNumber)
            .TupleOrError(town)
            .TupleOrError(postcode)
            .TupleOrError(coordinates)
            .Map(t => new Address(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5));

        return address;
    }
};
