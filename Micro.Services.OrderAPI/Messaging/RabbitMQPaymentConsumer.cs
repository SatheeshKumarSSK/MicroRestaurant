using Micro.Services.OrderAPI.Messages;
using Micro.Services.OrderAPI.Repository;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Micro.Services.EmailAPI.Messaging
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly string exchangeName;
        private readonly string queueName;
        private readonly string paymentOrderUpdateQueueName;
        private readonly OrderRepository _orderRepository;
        private readonly IConfiguration _config;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQPaymentConsumer(IConfiguration config, OrderRepository orderRepository)
        {
            _config = config;
            _orderRepository = orderRepository;

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            //exchangeName = _config.GetValue<string>("RabbitMQ:FanoutExchange");
            exchangeName = _config.GetValue<string>("RabbitMQ:DirectExchange");
            paymentOrderUpdateQueueName = _config.GetValue<string>("RabbitMQ:PaymentOrderUpdateQueueName");

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            //_channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, false);
            //queueName = _channel.QueueDeclare().QueueName;
            //_channel.QueueBind(queueName, exchangeName, "", null);

            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, false);
            _channel.QueueDeclare(paymentOrderUpdateQueueName, false, false, false, null);
            _channel.QueueBind(paymentOrderUpdateQueueName, exchangeName, "PaymentOrder");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                UpdatePaymentResultMessage updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(content);
                HandleMessage(updatePaymentResultMessage).GetAwaiter().GetResult();

                _channel.BasicAck(ea.DeliveryTag, false);
            };
            //_channel.BasicConsume(queueName, false, consumer);
            _channel.BasicConsume(paymentOrderUpdateQueueName, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(UpdatePaymentResultMessage message)
        {
            try
            {
                await _orderRepository.UpdateOrderPaymentStatus(message.OrderId, message.Status);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
