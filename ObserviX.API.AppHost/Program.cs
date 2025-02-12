var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var serviceBus = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureServiceBus("servicebus").AddQueue("observix-queue").AddQueue("observix-visitors-queue")
    : builder.AddConnectionString("servicebus");

// var keycloak = builder.AddKeycloak("observix-keycloak", 8080)
//     .WithDataVolume("observix-keycloak-data")
//     .WithExternalHttpEndpoints();

var collector = builder.AddProject<Projects.ObserviX_Collector>("observix-collector")
    .WithReference(cache)
    .WithReference(serviceBus)
    .WaitFor(cache);

builder.AddProject<Projects.ObserviX_Gateway>("observix-gateway")
    .WithExternalHttpEndpoints()
    .WithReference(collector)
    // .WithReference(keycloak)
    // .WaitFor(keycloak)
    .WaitFor(collector);


await builder.Build().RunAsync();