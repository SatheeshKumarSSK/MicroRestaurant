using Azure.Messaging.ServiceBus;
using Micro.MessageBus;
using Micro.Services.PaymentAPI.Messages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PaymentProcessor;
using System.Text;

namespace Micro.Services.PaymentAPI.Messaging
{
    public class AzureServiceBusConsumerPayment : IAzureServiceBusConsumerPayment
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderPaymentProcessTopic;
        private readonly string subscriptionPayment;
        private readonly string orderUpdatePaymentResultTopic;
        private readonly IConfiguration _config;
        private readonly IMessageBus _messageBus;
        private readonly IProcessPayment _processPayment;
        private ServiceBusProcessor orderPaymentProcessor;

        public AzureServiceBusConsumerPayment(IConfiguration config, IMessageBus messageBus, IProcessPayment processPayment)
        {
            _config = config;
            _messageBus = messageBus;
            _processPayment = processPayment;
            serviceBusConnectionString = _config.GetValue<string>("ServiceBus:ServiceBusConnectionString");
            orderPaymentProcessTopic = _config.GetValue<string>("ServiceBus:OrderPaymentProcessTopic");
            subscriptionPayment = _config.GetValue<string>("ServiceBus:OrderPaymentProcessSubscription");
            orderUpdatePaymentResultTopic = _config.GetValue<string>("ServiceBus:OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);
            orderPaymentProcessor = client.CreateProcessor(orderPaymentProcessTopic, subscriptionPayment);
        }

        public async Task Start()
        {
            orderPaymentProcessor.ProcessMessageAsync += ProcessPayments;
            orderPaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await orderPaymentProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await orderPaymentProcessor.StopProcessingAsync();
            await orderPaymentProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task ProcessPayments(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);

            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage updatePaymentResultMessage = new()
            {
                Status = result,
                OrderId = paymentRequestMessage.OrderId,
                Email = paymentRequestMessage.Email
            };

            try
            {
                await _messageBus.PublishMessage(updatePaymentResultMessage, orderUpdatePaymentResultTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
