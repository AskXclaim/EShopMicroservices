using CarterModule = BuildingBlocks.Utilities.CarterModule;

const string databaseConnectionStringName = "Database";

var builder = WebApplication.CreateBuilder(args);
//Add service to container

var carterModules = CarterModule.GetICarterModules(typeof(Program).Assembly);

builder.Services.AddCarter(configurator: c => { c.WithModules(carterModules.ToArray()); });
builder.Services.AddMediatR(config => { config.RegisterServicesFromAssembly(typeof(Program).Assembly); });
builder.Services.AddMarten(option =>
{
    option.Connection(builder.Configuration.GetConnectionString(databaseConnectionStringName)!);
}).UseLightweightSessions();

var app = builder.Build();
//configure http request pipeline
app.MapCarter();

await app.RunAsync();