using System.Text.Json;
using ObserviX.Gateway.Models;
using ObserviX.Shared.Exceptions;

namespace ObserviX.Gateway.Extensions
{
    public static class ReverseProxyConfigurationExtensions
    {
        public static void AddConfiguredReverseProxy(this IServiceCollection services, IConfiguration configuration,
            IHostEnvironment environment)
        {
            if (environment.IsEnvironment("Local"))
            {
                services.AddReverseProxy()
                    .LoadFromConfig(configuration.GetSection("ReverseProxy"));
            }
            else
            {
                var azureAppConfigReverseProxyStr = configuration.GetValue<string>("ReverseProxy");
                if (string.IsNullOrWhiteSpace(azureAppConfigReverseProxyStr))
                {
                    throw new ConfigurationException("AzureAppConfigurationReverseProxyConfig is missing in configuration.", "ReverseProxy");
                }

                var azureAppConfigReverseProxy = JsonSerializer.Deserialize<ReverseProxyConfiguration>(azureAppConfigReverseProxyStr);
                if (azureAppConfigReverseProxy == null)
                {
                    throw new ConfigurationException("Failed to parse ReverseProxy configuration.", "ReverseProxy");
                }

                services.AddReverseProxy()
                    .LoadFromMemory(azureAppConfigReverseProxy.GetRoutesConfigList(), azureAppConfigReverseProxy.GetClustersConfigList());
            }
        }
    }
}