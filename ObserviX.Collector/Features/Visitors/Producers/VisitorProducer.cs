using System.Text.Json;
using Azure.Messaging.ServiceBus;
using ObserviX.Shared.Interfaces;

namespace ObserviX.Collector.Features.Visitors.Producers
{
    public class VisitorProducer : IVisitorProducer
    {
        private readonly ServiceBusSender _sender;
        private readonly ILogger<VisitorProducer> _logger;
        private const string QueueName = "observix-visitors-queue";

        public VisitorProducer(ServiceBusClient serviceBusClient, ILogger<VisitorProducer> logger)
        {
            _sender = serviceBusClient.CreateSender(QueueName);
            _logger = logger;
        }

        public async Task SendMessage<T>(Guid tenantId, T data,
            CancellationToken cancellationToken = default)
        {
            var jsonData = JsonSerializer.Serialize(data);
            var message = new ServiceBusMessage(jsonData)
            {
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString()
            };
            message.ApplicationProperties["TenantId"] = tenantId;
            await _sender.SendMessageAsync(message, cancellationToken);
            _logger.LogInformation("Sent to queue '{Queue}' (TenantId: {TenantId}, MsgId: {MsgId})",
                QueueName, tenantId, message.MessageId);
        }

        public async ValueTask DisposeAsync()
        {
            await _sender.DisposeAsync();
        }
    }
}