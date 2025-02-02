using ObserviX.API.ServiceDefaults;
using ObserviX.Shared.Extensions.Caching;
using ObserviX.Shared.Extensions.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRedisOutputCacheWithPolicies();
builder.AddLoggingAndTelemetry(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseSerilogRequestLogging();
app.MapOpenApi();
app.UseHttpsRedirection();
app.UseOutputCache();

app.MapGet("/collector-test", () => 
    {
        Console.WriteLine("Request processed");
        return "Hello World! collector";
    })
    .WithName("test")
    .CacheOutput(CachingConstants.ProductsKey);

await app.RunAsync();