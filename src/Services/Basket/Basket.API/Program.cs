using CarterModule = BuildingBlocks.Utilities.CarterModule;

const string Database = "Database";

var builder = WebApplication.CreateBuilder(args);
//Add service to container
var assembly = typeof(Program).Assembly;
var carterModules = CarterModule.GetICarterModules(assembly);
builder.Services.AddCarter(configurator: c => { c.WithModules(carterModules.ToArray()); });

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddMarten(option =>
{
    option.Connection(builder.Configuration.GetConnectionString(Database)!);
    option.Schema.For<ShoppingCart>().Identity(cart => cart.UserName);
});

var app = builder.Build();

//configure http request pipeline
app.MapCarter();

await app.RunAsync();