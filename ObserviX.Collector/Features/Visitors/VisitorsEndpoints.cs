using MediatR;
using Microsoft.AspNetCore.Mvc;
using ObserviX.Collector.Features.Visitors.Commands;
using ObserviX.Collector.Features.Visitors.Queries;

namespace ObserviX.Collector.Features.Visitors
{
    public static class VisitorsEndpoints
    {
        public static IEndpointRouteBuilder MapVisitorEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/api/visitors", async (
                [FromServices] IMediator mediator,
                [FromBody] object? visitor,
                HttpContext context,
                CancellationToken ct) =>
            {
                var tenantId = (Guid)context.Items["TenantId"]!;
                if (visitor == null)
                    return Results.BadRequest("Visitor data is required.");

                await mediator.Send(new CreateVisitorCommand(tenantId, visitor), ct);
                return Results.Accepted();
            })
            .WithOpenApi()
            .AllowAnonymous();

            endpoints.MapGet("/api/visitors", async (
                [FromServices] IMediator mediator,
                HttpContext context,
                CancellationToken ct) =>
            {
                var tenantId = (Guid)context.Items["TenantId"]!;
                var result = await mediator.Send(new GetVisitorsQuery(tenantId), ct);
                return Results.Ok(result);
            })
            .WithOpenApi()
            .AllowAnonymous();

            endpoints.MapGet("/api/visitors/{visitorId:guid}", async (
                [FromServices] IMediator mediator,
                Guid visitorId,
                HttpContext context,
                CancellationToken ct) =>
            {
                var tenantId = (Guid)context.Items["TenantId"]!;
                var result = await mediator.Send(new GetVisitorByIdQuery(tenantId, visitorId), ct);
                return result is not null ? Results.Ok(result) : Results.NotFound();
            })
            .WithOpenApi()
            .AllowAnonymous();
            
            
            return endpoints;
        }
    }
}
