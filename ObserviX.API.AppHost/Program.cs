var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var collector = builder.AddProject<Projects.ObserviX_Collector>("observix-collector")
    .WithReference(cache)
    .WaitFor(cache);

builder.AddProject<Projects.ObserviX_Gateway>("observix-gateway")
    .WithExternalHttpEndpoints()
    .WithReference(collector)
    .WaitFor(collector);


await builder.Build().RunAsync();