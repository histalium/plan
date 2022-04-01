using OneOf;

namespace Employees.Logic;

public record FamilyName
{
    private FamilyName(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static OneOf<FamilyName, ErrorMessage> ParseFamilyName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ErrorMessage("Family name can not be empty.");
        }

        return new FamilyName(value);
    }
}
