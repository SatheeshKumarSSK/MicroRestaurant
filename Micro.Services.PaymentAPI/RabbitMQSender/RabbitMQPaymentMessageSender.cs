using Micro.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Micro.Services.PaymentAPI.RabbitMQSender
{
    public class RabbitMQPaymentMessageSender : IRabbitMQPaymentMessageSender
    {
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection;
        private readonly string exchangeName;
        private readonly string paymentEmailUpdateQueueName;
        private readonly string paymentOrderUpdateQueueName;
        private readonly IConfiguration _config;

        public RabbitMQPaymentMessageSender(IConfiguration config)
        {
            _hostname = "localhost";
            _password = "guest";
            _username = "guest";
            _config = config;
            //exchangeName = _config.GetValue<string>("RabbitMQ:FanoutExchange");
            exchangeName = _config.GetValue<string>("RabbitMQ:DirectExchange");
            paymentEmailUpdateQueueName = _config.GetValue<string>("RabbitMQ:PaymentEmailUpdateQueueName");
            paymentOrderUpdateQueueName = _config.GetValue<string>("RabbitMQ:PaymentOrderUpdateQueueName");
        }

        public void SendMessage(BaseMessage message)
        {
            if (ConnectionExists())
            {
                using var channel = _connection.CreateModel();
                //channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, false);
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, false);

                channel.QueueDeclare(paymentEmailUpdateQueueName, false, false, false, null);
                channel.QueueDeclare(paymentOrderUpdateQueueName, false, false, false, null);

                channel.QueueBind(paymentEmailUpdateQueueName, exchangeName, "PaymentEmail", null);
                channel.QueueBind(paymentOrderUpdateQueueName, exchangeName, "PaymentOrder", null);

                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                //channel.BasicPublish(exchangeName, "", null, body);
                channel.BasicPublish(exchangeName, "PaymentEmail", null, body);
                channel.BasicPublish(exchangeName, "PaymentOrder", null, body);
            }
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                //log exception
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }
            CreateConnection();
            return _connection != null;
        }
    }
}
