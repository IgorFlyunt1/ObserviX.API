using Azure.Messaging.ServiceBus;
using ObserviX.API.ServiceDefaults;
using ObserviX.Collector.Features.Visitors;
using ObserviX.Collector.Features.Visitors.Producers;
using ObserviX.Shared;
using ObserviX.Shared.Extensions.ApiResponseWrapper;
using ObserviX.Shared.Extensions.Caching;
using ObserviX.Shared.Interfaces;
using ObserviX.Shared.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var serviceName = builder.Configuration["SERVICE_NAME"]!;
var connectionString = builder.Configuration["ServiceBus:ConnectionString"];
builder.AddSharedServices(typeof(Program).Assembly, serviceName);
builder.Services.AddSingleton<ServiceBusClient>(_ =>
    new ServiceBusClient(builder.Configuration["ServiceBus:ConnectionString"]));
builder.Services.AddSingleton<IVisitorProducer, VisitorProducer>();


var app = builder.Build();
app.UseMiddleware<TenantValidationMiddleware>();
app.AddSharedPipeline(serviceName);
app.MapDefaultEndpoints();
var endpoints = app.MapGroup("")
    .AddEndpointFilter<ApiResponseWrapperFilter>();
endpoints.MapVisitorEndpoints();



app.MapGet("/collector-test", () => 
    {
        Console.WriteLine("Request processed");
        return $"Hello World! collector, {connectionString}";
    })
    .WithName("test")
    .CacheOutput(CachingConstants.ProductsKey)
    .WithOpenApi();

await app.RunAsync();