var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

builder.AddProject<Projects.ObserviX_Gateway>("observix-gateway")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.ObserviX_Collector>("observix-collector")
    .WithReference(cache)
    .WaitFor(cache);

await builder.Build().RunAsync();

