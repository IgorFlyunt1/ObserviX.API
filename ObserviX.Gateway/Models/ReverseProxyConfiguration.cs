using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;

namespace ObserviX.Gateway.Models;

public class ReverseProxyConfiguration
{
    public Dictionary<string, ReverseProxyRoute> Routes { get; set; } = new();
    public Dictionary<string, ReverseProxyCluster> Clusters { get; set; } = new();

    public List<RouteConfig> GetRoutesConfigList()
    {
        return Routes.Select(route => 
            new RouteConfig
            {
                RouteId = route.Key, 
                ClusterId = route.Value.ClusterId, 
                Match = new RouteMatch
                {
                    Path = route.Value.Match.Path
                }, 
                Transforms = route.Value.Transforms
            }).ToList();
    }
    
    public List<ClusterConfig> GetClustersConfigList()
    {
        return Clusters.Select(cluster => 
            new ClusterConfig
            {
                ClusterId = cluster.Key, 
                LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin, 
                Destinations = cluster.Value.Destinations.ToDictionary(
                    dest =>
                        dest.Key, 
                    dest =>
                        new DestinationConfig
                        {
                            Address = dest.Value.Address,
                        }
                    )
            }).ToList();
    }
}

public class ReverseProxyRoute
{
    public string ClusterId { get; set; } = string.Empty;
    public ReverseProxyMatch Match { get; set; } = new();
    public List<Dictionary<string, string>> Transforms { get; set; } = [];
}

public class ReverseProxyMatch
{
    public string Path { get; set; } = string.Empty;
}

public class ReverseProxyCluster
{
    public Dictionary<string, ReverseProxyDestination> Destinations { get; set; } = new();
}

public class ReverseProxyDestination
{
    public string Address { get; set; } = string.Empty;
}
