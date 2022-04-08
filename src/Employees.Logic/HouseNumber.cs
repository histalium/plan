namespace Employees.Logic;

public record HouseNumber
{
    private HouseNumber(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static OneOf<HouseNumber, ErrorMessage> ParseHouseNumber(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ErrorMessage("House number can not be empty.");
        }

        return new HouseNumber(value);
    }
}
