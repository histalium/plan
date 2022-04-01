namespace Employees.Logic;

public record GivenName
{
    private GivenName(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static OneOf<GivenName, ErrorMessage> ParseGivenName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ErrorMessage("Given name can not be empty.");
        }

        return new GivenName(value);
    }
}
