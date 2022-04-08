namespace Employees.Api;

internal class GetEmployee
{
    public Guid Id { get; set; }
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public GetAddress? Address { get; set; }
}
