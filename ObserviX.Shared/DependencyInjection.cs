using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ObserviX.Shared.Exceptions;
using ObserviX.Shared.Extensions.Caching;
using ObserviX.Shared.Extensions.Configuration;
using ObserviX.Shared.Extensions.Logging;
using ObserviX.Shared.Extensions.Mediatr;
using ObserviX.Shared.Extensions.Scalar;
using ObserviX.Shared.Middlewares;
using Serilog;

namespace ObserviX.Shared;

 public static class DependencyInjection
    {
        public static WebApplicationBuilder AddSharedServices(this WebApplicationBuilder builder, Assembly assembly)
        {
            builder.AddCustomConfiguration();
            builder.AddRedisOutputCacheWithPolicies();
            builder.AddLoggingAndTelemetry(builder.Configuration);
            builder.Services.AddOpenApi();
            builder.Services.AddHealthChecks();
            builder.Services.AddMediatrServices(assembly);
            
            
            // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //     .AddJwtBearer(options =>
            //     {
            //         options.Authority = builder.Configuration["Authentication:Authority"];
            //         options.Audience = builder.Configuration["Authentication:Audience"];
            //     });
            //
            // builder.Services.AddCors(options =>
            // {
            //     options.AddPolicy("AllowAll", policy =>
            //     {
            //         policy.AllowAnyOrigin()
            //               .AllowAnyHeader()
            //               .AllowAnyMethod();
            //     });
            // });

            // Optionally add other services (e.g., OpenTelemetry, Polly-based resilience, etc.)
            // builder.Services.AddOpenTelemetryTracing(...);
            // builder.Services.AddHttpClient("MyService").AddTransientHttpErrorPolicy(...);

            return builder;
        }

        // Extension method for WebApplication to configure the middleware pipeline.
        public static WebApplication AddSharedPipeline(this WebApplication app, string serviceName)
        {
            // if (!app.Environment.IsDevelopment() || !app.Environment.IsEnvironment("Local"))
            // {
            //     app.UseHsts();
            // }
            app.UseHsts();

            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseRouting();
            // app.UseCors("AllowAll");
            // app.UseAuthentication();
            // app.UseAuthorization();
            app.ConfigureScalarApi(serviceName!);
            app.UseCustomConfiguration();
            app.UseOutputCache();
            app.UseHealthChecks("/health");

            // if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
            // {
            //     app.MapOpenApi();
            // }
            //
            app.MapOpenApi();


            return app;
        }
    }