var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

// Add Aspire Service Bus component
var serviceBus = builder.AddAzureServiceBus("servicebus").AddQueue("observix-visitors-queue");

var collector = builder.AddProject<Projects.ObserviX_Collector>("observix-collector")
    .WithReference(cache)
    .WithReference(serviceBus)
    .WaitFor(cache)
    .WaitFor(serviceBus);

builder.AddProject<Projects.ObserviX_Gateway>("observix-gateway")
    .WithExternalHttpEndpoints()
    .WithReference(collector)
    .WaitFor(collector);


await builder.Build().RunAsync();