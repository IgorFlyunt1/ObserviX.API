using Azure.Identity;
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
        /// Loads local JSON files, environment variables, and Azure App Configuration via Service Connector.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder instance.</param>
        /// <param name="serviceLabel">
        /// The label to use when loading Azure App Configuration key-values.
        /// If provided, keys with this label will be loaded in addition to keys with no label.
        /// </param>
        /// <returns>The same WebApplicationBuilder instance for chaining.</returns>
        public static WebApplicationBuilder AddCustomConfiguration(this WebApplicationBuilder builder,
            string serviceLabel)
        {
            var env = builder.Environment;

            // Load JSON configuration files.
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (env.IsEnvironment("Local"))
            {
                builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
            }

            var appConfigEndpoint = "https://app-configuration-dev-observix.azconfig.io";

            if (!string.IsNullOrWhiteSpace(appConfigEndpoint))
            {
                try
                {
                    var credential = new DefaultAzureCredential();

                    builder.Configuration.AddAzureAppConfiguration(options =>
                    {
                        options.Connect(new Uri(appConfigEndpoint), credential)
                            // Load all keys with no label.
                            .Select(KeyFilter.Any);

                        // If a service label is provided, also load keys with that label.
                        if (!string.IsNullOrWhiteSpace(serviceLabel))
                        {
                            options.Select(KeyFilter.Any, serviceLabel);
                        }

                        // Configure the refresh mechanism.
                        options.ConfigureRefresh(refreshOptions =>
                        {
                            // This key (e.g., "RefreshTrigger") can be updated in App Configuration to trigger a refresh.
                            refreshOptions.Register("RefreshTrigger", refreshAll: true);
                            refreshOptions.SetRefreshInterval(TimeSpan.FromMinutes(1));
                        });
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return builder;
        }

        /// <summary>
        /// Configures the application to use Azure App Configuration refresh middleware if available.
        /// </summary>
        /// <param name="app">The WebApplication instance.</param>
        /// <returns>The same WebApplication instance for chaining.</returns>
        public static WebApplication UseCustomConfiguration(this WebApplication app)
        {
            var appConfigEndpoint = "https://app-configuration-dev-observix.azconfig.io";

            if (!string.IsNullOrWhiteSpace(appConfigEndpoint))
            {
                app.UseAzureAppConfiguration();
            }

            return app;
        }
    }
}