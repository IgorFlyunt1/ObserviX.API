namespace ObserviX.Collector.Features.Visitors.Queries;

public record GetVisitorByIdQuery(Guid TenantId, Guid VisitorId);