using OrderService.Api.DTOs;
using RabbitMQ.Client;
using Serilog;
using System.Text;
using System.Text.Json;

namespace OrderService.Api.RabbitMQ
{
    public class OrderPublisher
    {
        private readonly IRabbitMQConnection _connection;

        public OrderPublisher(IRabbitMQConnection connection)
        {
            _connection = connection;
        }

        public void PublishOrderCreated(OrderMessage orderMessage)
        {
            try
            {
                using var channel = _connection.CreateModel();
                channel.QueueDeclare(queue: "orders", durable: true, exclusive: false, autoDelete: false);

                var message = JsonSerializer.Serialize(orderMessage);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;  // Mesajın kalıcı olmasını sağlıyoruz

                channel.BasicPublish(exchange: "", routingKey: "orders", basicProperties: properties, body: body);

                Log.Information($"Order message sent: {orderMessage.OrderId}");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to publish order message: {ex.Message}");
            }
        }
    }
}
