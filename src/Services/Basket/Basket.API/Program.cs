using Basket.API.Constants;
using Basket.API.Data;
using Basket.API.Data.Instances;
using Basket.API.Data.Interfaces;
using BuildingBlocks.Exceptions.Handler;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using CarterModule = BuildingBlocks.Utilities.CarterModule;

const string Database = "Database";
const string DistributedCache = "DistributedCache";

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
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString(DistributedCache);
});
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString(Database)!)
    .AddRedis(DistributedCache);


var app = builder.Build();

//configure http request pipeline
app.MapCarter();
app.UseExceptionHandler(options => { });
app.UseHealthChecks($"/{Constants.BasketApiHealthCheckRoute}",
    new HealthCheckOptions()
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

await app.RunAsync();