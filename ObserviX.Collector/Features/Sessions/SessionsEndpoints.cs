// // SessionsEndpoints.cs
// using MediatR;
// using Microsoft.AspNetCore.Mvc;
// using ObserviX.Collector.Features.Commands;
// using ObserviX.Collector.Features.Sessions.Commands;
// using ObserviX.Collector.Features.Sessions.Queries;
//
// namespace ObserviX.Collector.Features.Sessions
// {
//     public static class SessionsEndpoints
//     {
//         public static IEndpointRouteBuilder MapSessionEndpoints(this IEndpointRouteBuilder endpoints)
//         {
//             endpoints.MapPost("/api/visitors/{visitorId:guid}/sessions", async (
//                 [FromServices] IMediator mediator,
//                 Guid visitorId,
//                 [FromBody] object? session,
//                 HttpContext context,
//                 CancellationToken ct) =>
//             {
//                 var tenantId = (Guid)context.Items["TenantId"]!;
//                 if (session == null)
//                     return Results.BadRequest("Session data is required.");
//                 
//                 await mediator.Send(new CreateSessionCommand(tenantId, visitorId, session), ct);
//                 return Results.Created($"/api/visitors/{visitorId}/sessions", session);
//             })
//             .WithOpenApi()
//             .AllowAnonymous();
//
//             endpoints.MapGet("/api/visitors/{visitorId:guid}/sessions", async (
//                 [FromServices] IMediator mediator,
//                 Guid visitorId,
//                 HttpContext context,
//                 CancellationToken ct) =>
//             {
//                 var tenantId = (Guid)context.Items["TenantId"]!;
//                 var result = await mediator.Send(new GetSessionsByVisitorQuery(tenantId, visitorId), ct);
//                 return Results.Ok(result);
//             })
//             .WithOpenApi()
//             .AllowAnonymous();
//
//             endpoints.MapGet("/api/visitors/{visitorId:guid}/sessions/{sessionId:guid}", async (
//                 [FromServices] IMediator mediator,
//                 Guid visitorId,
//                 Guid sessionId,
//                 HttpContext context,
//                 CancellationToken ct) =>
//             {
//                 var tenantId = (Guid)context.Items["TenantId"]!;
//                 var result = await mediator.Send(new GetSessionByIdQuery(tenantId, visitorId, sessionId), ct);
//                 return result is not null ? Results.Ok(result) : Results.NotFound();
//             })
//             .WithOpenApi()
//             .AllowAnonymous();
//
//             endpoints.MapPut("/api/visitors/{visitorId:guid}/sessions/{sessionId:guid}", async (
//                 [FromServices] IMediator mediator,
//                 Guid visitorId,
//                 Guid sessionId,
//                 [FromBody] object? updatedSession,
//                 HttpContext context,
//                 CancellationToken ct) =>
//             {
//                 if (updatedSession == null)
//                     return Results.BadRequest("Session data is required.");
//
//                 var tenantId = (Guid)context.Items["TenantId"]!;
//                 var success = await mediator.Send(new UpdateSessionCommand(tenantId, visitorId, sessionId, updatedSession), ct);
//                 return success ? Results.NoContent() : Results.NotFound();
//             })
//             .WithOpenApi()
//             .AllowAnonymous();
//
//             endpoints.MapDelete("/api/visitors/{visitorId:guid}/sessions/{sessionId:guid}", async (
//                 [FromServices] IMediator mediator,
//                 Guid visitorId,
//                 Guid sessionId,
//                 HttpContext context,
//                 CancellationToken ct) =>
//             {
//                 var tenantId = (Guid)context.Items["TenantId"]!;
//                 var success = await mediator.Send(new DeleteSessionCommand(tenantId, visitorId, sessionId), ct);
//                 return success ? Results.NoContent() : Results.NotFound();
//             })
//             .WithOpenApi()
//             .AllowAnonymous();
//
//             return endpoints;
//         }
//     }
// }
