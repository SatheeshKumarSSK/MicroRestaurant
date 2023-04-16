namespace Micro.Services.OrderAPI.Messaging
{
    public interface IAzureServiceBusConsumerOrder
    {
        public Task Start();
        public Task Stop();
    }
}
