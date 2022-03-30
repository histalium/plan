namespace Employees.Logic;

public record Postcode
{
    private Postcode(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static OneOf<Postcode, Error> ParsePostcode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error("Postcode can not be empty.");
        }

        return new Postcode(value);
    }
}
