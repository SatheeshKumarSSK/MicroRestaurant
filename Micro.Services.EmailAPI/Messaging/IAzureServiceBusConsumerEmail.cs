namespace Micro.Services.EmailAPI.Messaging
{
    public interface IAzureServiceBusConsumerEmail
    {
        public Task Start();
        public Task Stop();
    }
}
