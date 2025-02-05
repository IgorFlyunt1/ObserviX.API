using MediatR;
using Microsoft.AspNetCore.Mvc;
using ObserviX.Collector.Features.Visitors.Queries;
using ObserviX.Shared.Extensions.Caching;

namespace ObserviX.Collector.Features.Visitors;

public static class VisitorsEndpoints
{
    public const string GetVisitors = "/api/visitors";
    public const string GetVisitor = "/api/visitors/{id}";
    public const string CreateVisitor = "/api/visitors";
    public const string UpdateVisitor = "/api/visitors/{id}";
    public const string PatchVisitor = "/api/visitors/{id}";
    public const string DeleteVisitor = "/api/visitors/{id}";
    
    public const string GetVisitorVisits = "/api/visitors/{id}/visits";
    public const string GetVisitorVisit = "/api/visitors/{id}/visits/{visitId}";

    public static IEndpointRouteBuilder MapVisitorsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/visitors", async ([FromServices] IMediator mediator) =>
            await mediator.Send(new GetVisitorsQuery()))
            .WithOpenApi()
            .AllowAnonymous();

        // Add additional visitor endpoints as needed.
        return endpoints;
    }
}