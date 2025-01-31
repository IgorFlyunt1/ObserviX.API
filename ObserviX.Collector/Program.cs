using ObserviX.Shared.Caching;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRedisOutputCacheWithPolicies();

builder.Services.AddOpenApi();


var app = builder.Build();
app.MapDefaultEndpoints();
app.MapOpenApi();
app.UseHttpsRedirection();
app.UseOutputCache();


app.MapGet("/test", () => 
    {
        Console.WriteLine("Request processed");
        return "Hello World! collector";
    })
    .WithName("test")
    .CacheOutput(CachingConstants.ProductsKey);

await app.RunAsync();
