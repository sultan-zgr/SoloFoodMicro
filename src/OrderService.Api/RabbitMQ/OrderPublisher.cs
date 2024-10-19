using OrderService.Api.Models;
using RabbitMQ.Client;
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

        public void PublishOrderCreated(Order order)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue:"orders",durable:false,exclusive:false,autoDelete:false);

            var message = JsonSerializer.Serialize(order);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "orders", body: body);
        }
    }
}
