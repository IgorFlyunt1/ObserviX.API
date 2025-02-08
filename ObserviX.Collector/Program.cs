using ObserviX.API.ServiceDefaults;
using ObserviX.Collector.Features.Visitors;
using ObserviX.Collector.Features.Visitors.Queries;
using ObserviX.Shared;
using ObserviX.Shared.Extensions.ApiResponseWrapper;
using ObserviX.Shared.Extensions.Caching;

var builder = WebApplication.CreateBuilder(args);
builder.AddSharedServices(typeof(Program).Assembly);
builder.AddAzureServiceBusClient("servicebus");
var app = builder.Build();
var serviceName = app.Configuration.GetValue<string>("SERVICE_NAME")!;
app.AddSharedPipeline(serviceName);
app.MapDefaultEndpoints();
var endpoints = app.MapGroup("")
    .AddEndpointFilter<ApiResponseWrapperFilter>();
endpoints.MapVisitorsEndpoints();


app.MapGet("/collector-test", () =>
    {
        Console.WriteLine("Request processed");
        return "Hello World! collector";
    })
    .WithName("test")
    .CacheOutput(CachingConstants.ProductsKey)
    .WithOpenApi();

await app.RunAsync();