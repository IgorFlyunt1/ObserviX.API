using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ObserviX.Shared.Extensions.Cors;

public static class CorsExtensions
{
    private const string ProductionPolicyName = "ObserviXProductionCors";
    private const string DevelopmentPolicyName = "ObserviXDevelopmentCors";
    private const string CorsSettingsSection = "CorsSettings";

    public static IServiceCollection AddCustomCors(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var corsSettings = configuration.GetSection(CorsSettingsSection).Get<CorsSettings>();

        services.AddCors(options =>
        {
            options.AddPolicy(DevelopmentPolicyName, developmentPolicy =>
            {
                developmentPolicy.WithOrigins(corsSettings?.AllowedOrigins ?? [])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();

                if (corsSettings?.AllowedOrigins?.Any() != true)
                {
                    developmentPolicy.SetIsOriginAllowed(_ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            });

            options.AddPolicy(ProductionPolicyName, productionPolicy =>
            {
                if (corsSettings?.AllowedOrigins?.Any() == true)
                {
                    productionPolicy.WithOrigins(corsSettings.AllowedOrigins)
                        .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                        .WithHeaders(
                            "Authorization",
                            "Content-Type",
                            "X-Requested-With",
                            "X-Correlation-Id",
                            "ObserviX-Api-Version")
                        .AllowCredentials()
                        .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                }
                else
                {
                    // Fallback to strict policy if no origins configured
                    productionPolicy.DisallowCredentials()
                        .AllowAnyOrigin();
                }
            });

            options.DefaultPolicyName = environment.IsDevelopment() 
                ? DevelopmentPolicyName 
                : ProductionPolicyName;
        });

        return services;
    }

    public static WebApplication UseCustomCors(this WebApplication app)
    {
        var environment = app.Environment;
        var policyName = environment.IsDevelopment() 
            ? DevelopmentPolicyName 
            : ProductionPolicyName;

        app.UseCors(policyName);

        // Add exception for health checks endpoint
        app.Map("/health", healthApp =>
        {
            healthApp.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });

        return app;
    }
}

public class CorsSettings
{
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}