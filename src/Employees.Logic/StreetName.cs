namespace Employees.Logic;

public record StreetName
{
    private StreetName(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static OneOf<StreetName, Error> ParseStreetName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error("Street name can not be empty.");
        }

        return new StreetName(value);
    }
}
