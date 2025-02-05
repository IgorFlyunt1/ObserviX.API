var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var serviceBus = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureServiceBus("servicebus").AddQueue("observix-queue")
    : builder.AddConnectionString("servicebus");

var collector = builder.AddProject<Projects.ObserviX_Collector>("observix-collector")
    .WithReference(cache)
    .WithReference(serviceBus)
    .WaitFor(cache);

builder.AddProject<Projects.ObserviX_Gateway>("observix-gateway")
    .WithExternalHttpEndpoints()
    .WithReference(collector)
    .WaitFor(collector);


await builder.Build().RunAsync();