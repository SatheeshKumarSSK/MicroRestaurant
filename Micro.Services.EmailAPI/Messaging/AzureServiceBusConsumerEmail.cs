using Azure.Messaging.ServiceBus;
using Micro.Services.EmailAPI.Interfaces;
using Micro.Services.EmailAPI.Messages;
using Micro.Services.EmailAPI.Models;
using Micro.Services.EmailAPI.Repository;
using Newtonsoft.Json;
using System.Text;

namespace Micro.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumerEmail : IAzureServiceBusConsumerEmail
    {
        private readonly string serviceBusConnectionString;
        private readonly string subscriptionEmail;
        private readonly string orderUpdatePaymentResultTopic;

        private readonly EmailRepository _emailRepository;
        private readonly IConfiguration _config;

        private ServiceBusProcessor orderUpdatePaymentStatusProcessor;

        public AzureServiceBusConsumerEmail(EmailRepository emailRepository, IConfiguration config)
        {
            _emailRepository = emailRepository;
            _config = config;

            serviceBusConnectionString = _config.GetValue<string>("ServiceBus:ServiceBusConnectionString");
            subscriptionEmail = _config.GetValue<string>("ServiceBus:SubscriptionName");
            orderUpdatePaymentResultTopic = _config.GetValue<string>("ServiceBus:OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);

            orderUpdatePaymentStatusProcessor = client.CreateProcessor(orderUpdatePaymentResultTopic, subscriptionEmail);
        }

        public async Task Start()
        {
            orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
            await orderUpdatePaymentStatusProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await orderUpdatePaymentStatusProcessor.StopProcessingAsync();
            await orderUpdatePaymentStatusProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage objMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);
            try
            {
                await _emailRepository.SendAndLogEmail(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
