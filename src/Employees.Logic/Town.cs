namespace Employees.Logic;

public record Town
{
    private Town(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static OneOf<Town, Error> ParseTown(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error("Town can not be empty.");
        }

        return new Town(value);
    }
}
