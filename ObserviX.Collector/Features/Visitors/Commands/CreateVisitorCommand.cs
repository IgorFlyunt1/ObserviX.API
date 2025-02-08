using MediatR;
using ObserviX.Shared.Interfaces;

namespace ObserviX.Collector.Features.Visitors.Commands;

public sealed record CreateVisitorCommand(Guid TenantId, object Visitor) : IRequest;

public sealed class CreateVisitorHandler : IRequestHandler<CreateVisitorCommand>
{
    private readonly IVisitorProducer _visitorProducer;

    public CreateVisitorHandler(IVisitorProducer visitorProducer)
    {
        _visitorProducer = visitorProducer;
    }

    public async Task Handle(CreateVisitorCommand request, CancellationToken cancellationToken)
    {
        await _visitorProducer.SendMessage(request.TenantId, request.Visitor, cancellationToken);
    }
}