using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;
using Serilog.Sinks.SystemConsole.Themes;

namespace ObserviX.Shared.Extensions.Logging;

public static class LoggingAndTelemetryExtensions
{
    public static void AddLoggingAndTelemetry(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        AddSerilog(builder, configuration);
    }

    private static void AddSerilog(WebApplicationBuilder builder, IConfiguration configuration)
    {
        var levelSwitch = new Serilog.Core.LoggingLevelSwitch
        {
            // Default minimum level can be read from configuration if desired:
            MinimumLevel = configuration.GetValue("Logging:MinimumLevel", LogEventLevel.Debug)
        };

        builder.Host.UseSerilog((_, loggerConfiguration) =>
        {
            loggerConfiguration
                // Enrich logs with useful context information for microservices
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", configuration["ApplicationName"] ?? "UnknownApplication")
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithCorrelationId()
                .Enrich.WithExceptionDetails()

                // Filter out known noisy logs (health checks, framework noise, etc.)
                .Filter.ByExcluding(logEvent =>
                    Matching.FromSource("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware")(logEvent) ||
                    Matching.FromSource("Hangfire")(logEvent) ||
                    Matching.WithProperty<string>("RequestPath", path => path.Contains("/health"))(logEvent))

                .MinimumLevel.ControlledBy(levelSwitch)

                .WriteTo.Console(
                    theme: AnsiConsoleTheme.Code,
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

            // Add additional sinks for centralized logging
            AddElasticSearchLogging(configuration, loggerConfiguration);
            AddBetterStackLogging(configuration, loggerConfiguration);

            // Allow configuration from appsettings.json to override settings if needed
            loggerConfiguration.ReadFrom.Configuration(configuration);
        });
    }

    private static void AddElasticSearchLogging(IConfiguration configuration, LoggerConfiguration loggerConfiguration)
    {
        string? cloudId = configuration["ElasticSearch:CloudId"];
        string? apiKey = configuration["ElasticSearch:ApiKey"];
        if (!string.IsNullOrEmpty(cloudId) && !string.IsNullOrEmpty(apiKey))
        {
            loggerConfiguration.WriteTo.ElasticCloud(cloudId, apiKey,
                sinkOptions =>
                {
                    sinkOptions.DataStream = new DataStreamName(
                        "logs",
                        configuration["ElasticSearch:DataSet"]!,
                        configuration["ElasticSearch:Namespace"]!);
                });
        }
    }

    private static void AddBetterStackLogging(IConfiguration configuration, LoggerConfiguration loggerConfiguration)
    {
        string? betterStackSourceToken = configuration["BetterStack:SourceToken"];
        if (!string.IsNullOrEmpty(betterStackSourceToken))
        {
            loggerConfiguration.WriteTo.BetterStack(
                sourceToken: betterStackSourceToken
            );
        }
    }
}