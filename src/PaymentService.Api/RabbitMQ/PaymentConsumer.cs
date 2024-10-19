using OrderService.Api.Models;
using OrderService.Api.RabbitMQ;
using PaymentService.Api.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;
using System.Text.Json;

namespace PaymentService.Api.RabbitMQ
{
    public class PaymentConsumer
    {
        private readonly IRabbitMQConnection _connection;

        public PaymentConsumer(IRabbitMQConnection connection)
        {
            _connection = connection;
        }

        public void StartListening()
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: "orders", durable: false, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var order = JsonSerializer.Deserialize<OrderMessage>(message);

                Log.Information($"Processing payment for Order: {order.OrderId} - {order.Product}");
                var success = ProcessPayment(order);

                if (success)
                {
                    Log.Information($"Payment succeeded for Order: {order.OrderId}");
                }
                else
                {
                    Log.Error($"Payment failed for Order: {order.OrderId}");
                }
            };

            channel.BasicConsume(queue: "orders", autoAck: true, consumer: consumer);

            Log.Information("Waiting for orders.");
            Console.ReadLine();
        }

        private bool ProcessPayment(OrderMessage order)
        {
            bool success = new Random().Next(2) == 1;  // Ödeme işlemini simüle

            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: "order_updates", durable: false, exclusive: false, autoDelete: false);

            var updateMessage = JsonSerializer.Serialize(new OrderUpdateMessage
            {
                OrderId = order.OrderId,
                Status = success ? OrderStatus.Completed : OrderStatus.Failed
            });

            var body = Encoding.UTF8.GetBytes(updateMessage);
            channel.BasicPublish(exchange: "", routingKey: "order_updates", body: body);

            return success;
        }
    }
}
