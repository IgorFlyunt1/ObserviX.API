var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.ObserviX_API_ApiService>("apiservice");

builder.AddProject<Projects.ObserviX_API_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddProject<Projects.ObserviX_Gateway>("observix-gateway");

builder.AddProject<Projects.ObserviX_Collector>("observix-collector");

await builder.Build().RunAsync();

