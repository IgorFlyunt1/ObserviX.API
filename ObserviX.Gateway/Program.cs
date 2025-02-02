using System.Text.Json;
using ObserviX.API.ServiceDefaults;
using ObserviX.Gateway.Models;
using ObserviX.Shared.Extensions.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(builder.Configuration["AzureAppConfiguration:ConnectionString"]);
});

builder.AddServiceDefaults();
builder.AddLoggingAndTelemetry(builder.Configuration);
var azureAppConfigReverseProxyStr = builder.Configuration.GetValue<string>("AzureAppConfigurationReverseProxyConfig");
if (string.IsNullOrWhiteSpace(azureAppConfigReverseProxyStr))
{
    throw new Exception("Failed to get ReverseProxy configuration from Azure App Configuration Service.");
}

var azureAppConfigReverseProxy = JsonSerializer.Deserialize<ReverseProxyConfiguration>(azureAppConfigReverseProxyStr);
if (azureAppConfigReverseProxy == null)
{
    throw new Exception("Failed to parse ReverseProxy configuration.");
}

builder.Services.AddReverseProxy()
    .LoadFromMemory(azureAppConfigReverseProxy.GetRoutesConfigList(), azureAppConfigReverseProxy.GetClustersConfigList());

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapReverseProxy();

app.MapGet("/gateway-test", () => 
        $"Hello World! \nGateway - AppSettings proxy config: \n{builder.Configuration.GetSection("ReverseProxy").Value} \n Azure App Configuration proxy config: \n{azureAppConfigReverseProxyStr}")
    .WithName("test");


await app.RunAsync();