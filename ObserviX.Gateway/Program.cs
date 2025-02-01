var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(builder.Configuration["AzureAppConfiguration:ConnectionString"]);
});

builder.AddServiceDefaults();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var proxyValues = builder.Configuration.GetSection("ReverseProxy");

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapReverseProxy();

app.MapGet("/gateway-test", () => $"Hello World! Gateway - TestValue: {proxyValues.Value}")
    .WithName("test");


await app.RunAsync();