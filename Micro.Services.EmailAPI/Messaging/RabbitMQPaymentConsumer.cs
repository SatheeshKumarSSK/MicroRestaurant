using Micro.Services.EmailAPI.Messages;
using Micro.Services.EmailAPI.Repository;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Micro.Services.EmailAPI.Messaging
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly EmailRepository _emailRepository;
        private readonly string exchangeName;
        private readonly string queueName;
        private readonly string paymentEmailUpdateQueueName;
        private readonly IConfiguration _config;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQPaymentConsumer(IConfiguration config, EmailRepository emailRepository)
        {
            _config = config;
            _emailRepository = emailRepository;

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            //exchangeName = _config.GetValue<string>("RabbitMQ:FanoutExchange");
            exchangeName = _config.GetValue<string>("RabbitMQ:DirectExchange");
            paymentEmailUpdateQueueName = _config.GetValue<string>("RabbitMQ:PaymentEmailUpdateQueueName");

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            //_channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, false);
            //queueName = _channel.QueueDeclare().QueueName;
            //_channel.QueueBind(queueName, exchangeName, "", null);

            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, false);
            _channel.QueueDeclare(paymentEmailUpdateQueueName, false, false, false, null);
            _channel.QueueBind(paymentEmailUpdateQueueName, exchangeName, "PaymentEmail");
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
            _channel.BasicConsume(paymentEmailUpdateQueueName, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(UpdatePaymentResultMessage updatePaymentResultMessage)
        {
            try
            {
                await _emailRepository.SendAndLogEmail(updatePaymentResultMessage);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
