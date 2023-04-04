using AutoMapper;
using Azure.Messaging.ServiceBus;
using Micro.Services.OrderAPI.Interfaces;
using Micro.Services.OrderAPI.Messages;
using Micro.Services.OrderAPI.Models;
using Micro.Services.OrderAPI.Repository;
using Newtonsoft.Json;
using System.Text;

namespace Micro.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string checkoutMessageTopic;
        private readonly string subscriptionCheckOut;
        private readonly OrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private ServiceBusProcessor checkOutProcessor;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IMapper mapper, IConfiguration config)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _config = config;

            serviceBusConnectionString = config.GetValue<string>("ServiceBusConnectionString");
            checkoutMessageTopic = config.GetValue<string>("CheckoutMessageTopic");
            subscriptionCheckOut = config.GetValue<string>("SubscriptionCheckOut");

            var client = new ServiceBusClient(serviceBusConnectionString);
            checkOutProcessor = client.CreateProcessor(checkoutMessageTopic, subscriptionCheckOut);
        }

        public async Task Start()
        {
            checkOutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            checkOutProcessor.ProcessErrorAsync += ErrorHandler;
            await checkOutProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await checkOutProcessor.StopProcessingAsync();
            await checkOutProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

            OrderHeader orderHeader = _mapper.Map<OrderHeader>(checkoutHeaderDto);

            orderHeader.OrderDetails = new List<OrderDetail>();
            orderHeader.OrderTime = DateTime.Now;
            orderHeader.PaymentStatus = false;

            foreach (var detailList in checkoutHeaderDto.CartDetails)
            {
                OrderDetail orderDetails = new()
                {
                    ProductId = detailList.ProductId,
                    ProductName = detailList.Product.Name,
                    Price = detailList.Product.Price,
                    Count = detailList.Count
                };
                orderHeader.CartTotalItems += detailList.Count;
                orderHeader.OrderDetails.Add(orderDetails);
            }

            await _orderRepository.AddOrder(orderHeader);
        }
    }
}
