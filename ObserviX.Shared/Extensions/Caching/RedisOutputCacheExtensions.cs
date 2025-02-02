using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ObserviX.Shared.Extensions.Caching;

public static class RedisOutputCacheExtensions
{
    public static void AddRedisOutputCacheWithPolicies(this WebApplicationBuilder builder)
    {
        builder.AddRedisOutputCache("cache");
        builder.Services.AddOutputCache(options =>
        {
            AddProducts(options);
        });
    }
    
    private static void AddProducts(OutputCacheOptions options)
    {
        options.AddPolicy(CachingConstants.ProductsKey, build =>
            build.Expire(TimeSpan.FromDays(7))
                .Tag(CachingConstants.ProductsKey));
    }
}