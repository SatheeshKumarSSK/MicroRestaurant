using Micro.MessageBus;

namespace Micro.Services.OrderAPI.RabbitMQSender
{
    public interface IRabbitMQOrderMessageSender
    {
        void SendMessage(BaseMessage message, String queueName);
    }
}
