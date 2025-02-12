using ObserviX.API.ServiceDefaults;
using ObserviX.Gateway.Extensions;
using ObserviX.Shared.Extensions.Configuration;
using ObserviX.Shared.Extensions.Logging;
using ObserviX.Shared.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var serviceName = builder.Configuration["SERVICE_NAME"]!;
builder.AddCustomConfiguration(serviceName);
builder.AddServiceDefaults();
builder.AddLoggingAndTelemetry(builder.Configuration);
builder.Services.AddConfiguredReverseProxy(builder.Configuration, builder.Environment);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer(
        serviceName: "observix-keycloak",
        realm: "gateway",
        options =>
        {
            options.Audience = "observix.gateway";
            options.RequireHttpsMetadata = builder.Environment.IsProduction();
        });
builder.Services.AddAuthorizationBuilder();

var app = builder.Build();
if (!app.Environment.IsDevelopment() || !app.Environment.IsEnvironment("Local"))
{
    app.UseHsts();
}

app.UseCors("ConfiguredCors");
app.UseMiddleware<TenantExtractionMiddleware>();
app.UseCustomConfiguration();
app.MapDefaultEndpoints();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.MapReverseProxy();
app.UseAuthentication();
app.UseAuthorization();


var azureAppConfigConnectionFromEnvironment =
    Environment.GetEnvironmentVariable("AzureAppConfiguration__ConnectionString");

app.MapGet("/config", () =>
    {
        var result = new
        {
            // Azure App Config connection tested in various ways:
            azureAppConfigConnectionFromEnvironment = azureAppConfigConnectionFromEnvironment ?? "Not Found",
        };

        return Results.Json(result);
    })
    .WithName("config")
    .WithOpenApi();


app.MapGet("/gateway-test", () =>
    {
        if (app.Environment.IsEnvironment("Local"))
        {
            return
                $"Hello World! \nGateway - AppSettings proxy config: \n{builder.Configuration.GetSection("ReverseProxy").Value}";
        }

        var azureConfig = builder.Configuration.GetValue<string>("AzureAppConfigurationReverseProxyConfig");
        return $"Hello World! \nAzure App Configuration proxy config: \n{azureConfig}";
    })
    .WithName("test")
    .WithOpenApi()
    .RequireAuthorization();

await app.RunAsync();