namespace Employees.Logic;

public record Town
{
    private Town(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static OneOf<Town, ErrorMessage> ParseTown(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ErrorMessage("Town can not be empty.");
        }

        return new Town(value);
    }
}
