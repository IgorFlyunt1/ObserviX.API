using ObserviX.API.ServiceDefaults;
using ObserviX.Collector.Features;
using ObserviX.Collector.Features.Visitors;
using ObserviX.Shared;
using ObserviX.Shared.Exceptions;
using ObserviX.Shared.Extensions.ApiResponseWrapper;
using ObserviX.Shared.Extensions.Caching;
using ObserviX.Shared.Extensions.Configuration;
using ObserviX.Shared.Extensions.Logging;
using ObserviX.Shared.Extensions.Scalar;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.AddSharedServices();
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