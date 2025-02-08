namespace ObserviX.Shared.Interfaces
{
    public interface IVisitorProducer : IAsyncDisposable
    {
        Task SendMessage<T>(Guid tenantId, T data, CancellationToken cancellationToken = default);
    }
}
