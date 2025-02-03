using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;

namespace ObserviX.Shared.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Adds custom configuration sources to the WebApplicationBuilder.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder instance.</param>
        /// <returns>The same WebApplicationBuilder instance for chaining.</returns>
        public static WebApplicationBuilder AddCustomConfiguration(this WebApplicationBuilder builder)
        {
            var env = builder.Environment;

            builder.Configuration
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (env.IsEnvironment("Local"))
            {
                builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
            }

            builder.Configuration.AddEnvironmentVariables();

            var appConfigConnectionString = builder.Configuration["AppConfigConnectionString"];

            if (!string.IsNullOrWhiteSpace(appConfigConnectionString))
            {
                builder.Configuration.AddAzureAppConfiguration(options =>
                {
                    options.Connect(appConfigConnectionString)
                           .Select(KeyFilter.Any)
                           .ConfigureRefresh(refreshOptions =>
                           {
                               refreshOptions.Register("RefreshTrigger", refreshAll: true);
                               refreshOptions.SetRefreshInterval(TimeSpan.FromMinutes(1));
                           });
                });
            }

            return builder;
        }

        /// <summary>
        /// Configures the application to use Azure App Configuration refresh middleware if configured.
        /// </summary>
        /// <param name="app">The WebApplication instance.</param>
        /// <returns>The same WebApplication instance for chaining.</returns>
        public static WebApplication UseCustomConfiguration(this WebApplication app)
        {
            var appConfigConnectionString = app.Configuration["AppConfigConnectionString"];

            if (!string.IsNullOrWhiteSpace(appConfigConnectionString))
            {
                app.UseAzureAppConfiguration();
            }

            return app;
        }
    }
}
