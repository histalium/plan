namespace Employees.Logic;

public record HouseNumber
{
    private HouseNumber(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static OneOf<HouseNumber, Error> ParseHouseNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error("House number can not be empty.");
        }

        return new HouseNumber(value);
    }
}
