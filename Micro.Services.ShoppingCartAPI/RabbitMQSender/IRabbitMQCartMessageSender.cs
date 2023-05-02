using Micro.MessageBus;

namespace Micro.Services.ShoppingCartAPI.RabbitMQSender
{
    public interface IRabbitMQCartMessageSender
    {
        void SendMessage(BaseMessage message, String queueName);
    }
}
