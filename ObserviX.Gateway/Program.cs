var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(builder.Configuration["AzureAppConfiguration:ConnectionString"]);
});





builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


//get   "TestValue" : "TestValueCollector"  from Azure App Configuration
var testValue = builder.Configuration["TestValue"];
var proxyValues = builder.Configuration.GetSection("ReverseProxy");
var testValue2 = proxyValues;




var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapReverseProxy();

app.MapGet("/test", () => $"Hello World! Gateway 2 - TestValue: {proxyValues}")
    .WithName("test");


await app.RunAsync();