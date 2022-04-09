namespace Employees.Api;

public record Event<T>(Guid Stream, int Version, string Name, T Data);
