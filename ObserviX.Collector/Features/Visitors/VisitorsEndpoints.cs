using MediatR;
using Microsoft.AspNetCore.Mvc;

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

    public static void MapVisitorsEndpoints(this WebApplication app)
    {
   

    }
}