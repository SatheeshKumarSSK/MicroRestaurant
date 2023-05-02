using Micro.MessageBus;

namespace Micro.Services.PaymentAPI.RabbitMQSender
{
    public interface IRabbitMQPaymentMessageSender
    {
        void SendMessage(BaseMessage message);
    }
}
