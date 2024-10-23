using CarterModule = BuildingBlocks.Utilities.CarterModule;

const string databaseConnectionStringName = "Database";
const string developmentDbInitializationRetry = "Development_Db_Initialization_Retry";

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
    AddDevelopmentDbInitializationRetryStrategy(builder, developmentDbInitializationRetry);
    await using var provider = builder.Services.BuildServiceProvider();
    var pipelineProvider = provider.GetRequiredService<ResiliencePipelineProvider<string>>();
    var pipeline = pipelineProvider.GetPipeline(developmentDbInitializationRetry);
    await pipeline.ExecuteAsync(_ =>
    {
        builder.Services.InitializeMartenWith<CatalogInitialData>();

        return ValueTask.CompletedTask;
    });

    //builder.Services.InitializeMartenWith<CatalogInitialData>();
}

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString(databaseConnectionStringName)!);

var app = builder.Build();

//configure http request pipeline
app.MapCarter();
app.UseExceptionHandler(options => { });
app.UseHealthChecks($"/{Constants.CategoryApiHealthCheckRoute}", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

await app.RunAsync();

void AddDevelopmentDbInitializationRetryStrategy(WebApplicationBuilder webApplicationBuilder,
    string pipelineName)
{
    webApplicationBuilder.Services.AddResiliencePipeline(pipelineName, pipelineBuilder =>
    {
        pipelineBuilder.AddRetry(new RetryStrategyOptions()
        {
            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
            BackoffType = DelayBackoffType.Exponential,
            MaxRetryAttempts = 3,
            UseJitter = true,
            Delay = TimeSpan.FromSeconds(1),
        });
    });
}