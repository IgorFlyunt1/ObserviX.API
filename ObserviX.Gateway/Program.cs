using ObserviX.API.ServiceDefaults;
using ObserviX.Gateway.Extensions;
using ObserviX.Shared.Extensions.Configuration;
using ObserviX.Shared.Extensions.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomConfiguration();
builder.AddServiceDefaults();
builder.AddLoggingAndTelemetry(builder.Configuration);
builder.Services.AddConfiguredReverseProxy(builder.Configuration, builder.Environment);

var app = builder.Build();
if (!app.Environment.IsDevelopment() || !app.Environment.IsEnvironment("Local"))
{
    app.UseHsts();
}
app.UseCors("ConfiguredCors");
app.UseCustomConfiguration();
app.MapDefaultEndpoints();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.MapReverseProxy();

app.MapGet("/gateway-test", () =>
{
    if (app.Environment.IsEnvironment("Local"))
    {
        return $"Hello World! \nGateway - AppSettings proxy config: \n{builder.Configuration.GetSection("ReverseProxy").Value}";
    }
    else
    {
        var azureConfig = builder.Configuration.GetValue<string>("AzureAppConfigurationReverseProxyConfig");
        return $"Hello World! \nAzure App Configuration proxy config: \n{azureConfig}";
    }
}).WithName("test")
.WithOpenApi();

await app.RunAsync();