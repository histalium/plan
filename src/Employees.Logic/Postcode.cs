namespace Employees.Logic;

public record Postcode
{
    private Postcode(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static OneOf<Postcode, ErrorMessage> ParsePostcode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ErrorMessage("Postcode can not be empty.");
        }

        return new Postcode(value);
    }
}
