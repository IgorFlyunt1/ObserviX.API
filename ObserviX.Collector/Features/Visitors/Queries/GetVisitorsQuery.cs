using MediatR;
using ObserviX.Collector.Features.Visitors.Models;
using ObserviX.Shared.Entities;

namespace ObserviX.Collector.Features.Visitors.Queries;

public sealed record GetVisitorsQuery(Guid TenantId) : IRequest<IReadOnlyCollection<VisitorDto>>;

public sealed class GetVisitorsHandler : IRequestHandler <GetVisitorsQuery, IReadOnlyCollection<VisitorDto>>
{
    
    public async Task<IReadOnlyCollection<VisitorDto>> Handle(GetVisitorsQuery query, CancellationToken cancellationToken)
    {
        var visitors = new List<VisitorDto>
        {
            new VisitorDto
            {
                VisitorId = Guid.NewGuid()
            }
        };
        
        return visitors;
    }
}
