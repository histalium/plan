namespace Employees.Logic;

public record StreetName
{
    private StreetName(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static OneOf<StreetName, ErrorMessage> ParseStreetName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ErrorMessage("Street name can not be empty.");
        }

        return new StreetName(value);
    }
}
