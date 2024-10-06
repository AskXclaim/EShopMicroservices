using BuildingBlocks.Exceptions.Handler;
using Catalog.API.Data;
using Polly;
using Polly.Registry;
using Polly.Retry;
using CarterModule = BuildingBlocks.Utilities.CarterModule;

const string databaseConnectionStringName = "Database";
const string DevelopmentDbInitializationRetry = "Development_Db_Initialization_Retry";

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
    option.Connection(builder.Configuration.GetConnectionString(databaseConnectionStringName)!);
}).UseLightweightSessions();

if (builder.Environment.IsDevelopment())
{
    AddDevelopmentDbInitializationRetryStrategy(builder, DevelopmentDbInitializationRetry);
    await using var provider = builder.Services.BuildServiceProvider();
    var pipelineProvider = provider.GetRequiredService<ResiliencePipelineProvider<string>>();
    var pipeline = pipelineProvider.GetPipeline(DevelopmentDbInitializationRetry);
    await pipeline.ExecuteAsync(_ =>
    {
        builder.Services.InitializeMartenWith<CatalogInitialData>();

        return ValueTask.CompletedTask;
    });
}

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

//configure http request pipeline
app.MapCarter();

await app.RunAsync();

void AddDevelopmentDbInitializationRetryStrategy(WebApplicationBuilder webApplicationBuilder,
    string developmentDbInitializationRetry)
{
    webApplicationBuilder.Services.AddResiliencePipeline(developmentDbInitializationRetry, builder =>
    {
        builder.AddRetry(new RetryStrategyOptions()
        {
            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
            BackoffType = DelayBackoffType.Exponential,
            MaxRetryAttempts = 3,
            UseJitter = true,
            Delay = TimeSpan.FromSeconds(1),
        });
    });
}