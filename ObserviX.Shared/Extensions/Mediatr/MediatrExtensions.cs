using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ObserviX.Shared.Extensions.Mediatr;

public static class MediatrExtensions
{
    /// <summary>
    /// Registers MediatR along with production-ready pipeline behaviors.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="assembly"></param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMediatrServices(this IServiceCollection services, Assembly assembly) 
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
        });


        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}