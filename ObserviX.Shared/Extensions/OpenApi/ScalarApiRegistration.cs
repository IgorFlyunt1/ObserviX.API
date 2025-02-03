using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace ObserviX.Shared.Extensions.Scalar
{
    public class ScalarServerConfiguration
    {
        public string Url { get; set; } = string.Empty;
        public string? Name { get; set; }
    }

    public static class ScalarApiRegistration
    {
        public static void ConfigureScalarApi(this WebApplication app, string serviceProxyName)
        {
            var config = app.Services.GetRequiredService<IConfiguration>();
            var scalarSection = config.GetSection("Scalar");
            
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("ObserviX API")
                       .WithTheme(ScalarTheme.DeepSpace)
                       .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);

                var servers = scalarSection.GetSection("Servers").Get<List<ScalarServerConfiguration>>();
                if (servers is not { Count: > 0 })
                {
                    return;
                }

                foreach (var server in servers)
                {
                    if (!string.IsNullOrWhiteSpace(server.Name))
                    {
                        options.AddServer(new ScalarServer($"{server.Url}/{serviceProxyName}", server.Name));
                    }
                    else
                    {
                        options.AddServer($"{server.Url}/{serviceProxyName}");
                    }
                }
            });
        }
    }
}
