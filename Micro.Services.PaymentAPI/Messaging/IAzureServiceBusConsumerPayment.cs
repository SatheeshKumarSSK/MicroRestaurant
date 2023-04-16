namespace Micro.Services.PaymentAPI.Messaging
{
    public interface IAzureServiceBusConsumerPayment
    {
        public Task Start();
        public Task Stop();
    }
}
