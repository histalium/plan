using OneOf;

namespace Employees.Logic;

public record FamilyName
{
    private FamilyName(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static OneOf<FamilyName, Error> ParseFamilyName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error("Family name can not be empty.");
        }

        return new FamilyName(value);
    }
}
