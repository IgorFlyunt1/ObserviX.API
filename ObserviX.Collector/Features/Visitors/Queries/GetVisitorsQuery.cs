﻿using MediatR;
using ObserviX.Collector.Features.Visitors.Models;
using ObserviX.Shared.Entities;

namespace ObserviX.Collector.Features.Visitors.Queries;

public sealed record GetVisitorsQuery() : IRequest<ApiResponse<IReadOnlyCollection<VisitorDto>>>;

public sealed class GetVisitorsHandler : IRequestHandler <GetVisitorsQuery, ApiResponse<IReadOnlyCollection<VisitorDto>>>
{
    
    public async Task<ApiResponse<IReadOnlyCollection<VisitorDto>>> Handle(GetVisitorsQuery query, CancellationToken cancellationToken)
    {
        return ApiResponse<IReadOnlyCollection<VisitorDto>>.SuccessResponse(new List<VisitorDto>
        {
            new VisitorDto
            {
            }
        });
    }
}
