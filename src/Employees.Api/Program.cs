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
builder.Services.AddScoped<ChangeEmployeeAddress>();
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
                FamilyName = e.FamilyName.Value,
                Address = e.Address.Match<GetAddress?>(
                    a => new GetAddress
                    {
                        StreetName = a.StreetName.Value,
                        HouseNumber = a.HouseNumber.Value,
                        Town = a.Town.Value,
                        Postcode = a.Postcode.Value,
                        Coordinates = new GetCoordinates
                        {
                            Latitude = a.Coordinates.Latitude,
                            Longitude = a.Coordinates.Longitude
                        }
                    },
                    _ => null)
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

app.MapPut("/employees/{employeeId}/address", async ([FromBody] PutEmployeeAddress employeeAddress, [FromRoute] Guid employeeId,
    [FromServices] ChangeEmployeeAddress changeEmployeeAddress) =>
{
    var streetName = StreetName.ParseStreetName(employeeAddress.StreetName);
    var houseNumber = HouseNumber.ParseHouseNumber(employeeAddress.HouseNumber);
    var town = Town.ParseTown(employeeAddress.Town);
    var postcode = Postcode.ParsePostcode(employeeAddress.Postcode);
    var coordinates = Coordinates.ParseCoordinates(employeeAddress.Coordinates?.Latitude, employeeAddress.Coordinates?.Longitude);
    var id = EmployeeId.FromGuid(employeeId);
    var address = streetName
        .TupleOrError(houseNumber)
        .TupleOrError(town)
        .TupleOrError(postcode)
        .TupleOrError(coordinates)
        .Match<OneOf<Address, ErrorMessage>>(
            ((StreetName StreetName, HouseNumber HouseNumber, Town Town, Postcode Postcode, Coordinates Coordinates) t)
                => new Address(t.StreetName, t.HouseNumber, t.Town, t.Postcode, t.Coordinates),
            e => e
        );

    var createdResult = await address.Match<Task<OneOf<None, ErrorMessage>>>(
        t => changeEmployeeAddress.ExecuteAsync(id, t),
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
