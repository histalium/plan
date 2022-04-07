using Employees.Api;
using Employees.Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using OneOf;
using OneOf.Types;
using Plan.Utilities;

var builder = WebApplication.CreateBuilder(args);

var cosmosUri = builder.Configuration["Cosmos:Uri"];
builder.Services.AddSingleton<CosmosClient>((_) =>
{
    var cosmosUri = builder.Configuration["Cosmos:Uri"];
    if (string.IsNullOrEmpty(cosmosUri))
    {
        throw new Exception("Config 'Cosmos:Uri' is required");
    }
    var cosmosKey = builder.Configuration["Cosmos:Key"];
    if (string.IsNullOrEmpty(cosmosKey))
    {
        throw new Exception("Config 'Cosmos:Key' is required");
    }

    return new CosmosClient(cosmosUri, cosmosKey);
});

builder.Services.AddScoped<Employees.Logic.GetEmployee>();
builder.Services.AddScoped<CreateEmployee>();
builder.Services.AddScoped<ChangeEmployeeName>();
builder.Services.AddScoped<IEmployeeEventStore, EmployeeEventStore>();

var app = builder.Build();

app.MapGet("/employees/{employeeId}", async ([FromRoute] Guid employeeId, [FromServices] Employees.Logic.GetEmployee getEmployee) =>
{
    var Id = EmployeeId.FromGuid(employeeId);

    var getResult = await getEmployee.ExecuteAsync(Id);

    var respons = getResult
        .Match(
            e => Results.Ok(new Employees.Api.GetEmployee
            {
                Id = e.Id.Value,
                GivenName = e.GivenName.Value,
                FamilyName = e.FamilyName.Value
            }),
            _ => Results.NotFound(),
            e => Results.BadRequest(e)
        );

    return respons;
});

app.MapPost("/employees", async ([FromBody] PostEmployee employee, [FromServices] CreateEmployee createEmployee) =>
{
    var givenName = GivenName.ParseGivenName(employee.GivenName);
    var familyName = FamilyName.ParseFamilyName(employee.FamilyName);
    var name = givenName
        .TupleOrError(familyName);

    var createdResult = await name.Match<Task<OneOf<None, ErrorMessage>>>(
        t => createEmployee.ExecuteAsync(t.Item1, t.Item2),
        e => Task.FromResult((OneOf<None, ErrorMessage>)e)
    );

    var respons = createdResult
        .Match(
            _ => Results.Accepted(),
            e => Results.BadRequest(e)
        );

    return respons;
});

app.MapPut("/employees/{employeeId}/name", async ([FromBody] PutEmployeeName employeeName, [FromRoute] Guid employeeId,
    [FromServices] ChangeEmployeeName changeEmployeeName) =>
{
    var givenName = GivenName.ParseGivenName(employeeName.GivenName);
    var familyName = FamilyName.ParseFamilyName(employeeName.FamilyName);
    var id = EmployeeId.FromGuid(employeeId);
    var name = givenName
        .TupleOrError(familyName);

    var createdResult = await name.Match<Task<OneOf<None, ErrorMessage>>>(
        t => changeEmployeeName.ExecuteAsync(id, t.Item1, t.Item2),
        e => Task.FromResult((OneOf<None, ErrorMessage>)e)
    );

    var respons = createdResult
        .Match(
            _ => Results.Accepted(),
            e => Results.BadRequest(e)
        );

    return respons;
});

app.Run();
