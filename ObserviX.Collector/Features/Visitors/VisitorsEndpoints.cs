using MediatR;
using Microsoft.AspNetCore.Mvc;
using ObserviX.Collector.Features.Visitors.Commands;

namespace ObserviX.Collector.Features.Visitors;

public static class VisitorsEndpoints
{
    public static IEndpointRouteBuilder MapVisitorEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/visitors", async (
                [FromServices] IMediator mediator,
                [FromBody] object? visitor,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                var tenantId = (Guid)context.Items["TenantId"]!;
                if (visitor == null)
                {
                    return Results.BadRequest("Visitor data is required.");
                }
                
                await mediator.Send(new CreateVisitorCommand(tenantId, visitor), cancellationToken);

                return Results.Accepted();
            })
            .WithOpenApi() 
            .AllowAnonymous();

        return endpoints;
    }
}