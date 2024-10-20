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
            channel.QueueDeclare(queue: "orders", durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var orderMessage = JsonSerializer.Deserialize<OrderMessage>(message);
                    Log.Information($"Processing payment for Order: {orderMessage.OrderId} - {orderMessage.Product}");

                    var success = ProcessPayment(orderMessage);

                    PublishOrderUpdate(orderMessage, success);

                    // Mesajı başarıyla işledikten sonra onay gönder
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error processing message: {ex.Message}");
                    // Hata durumunda mesajı yeniden kuyruğa ekle
                    channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            channel.BasicConsume(queue: "orders", autoAck: false, consumer: consumer);
            Log.Information("PaymentConsumer is listening to RabbitMQ.");
        }

        private bool ProcessPayment(OrderMessage order)
        {
            return new Random().Next(2) == 1;  // Ödeme işlemini simüle ediyoruz
        }

        private void PublishOrderUpdate(OrderMessage order, bool success)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: "order_updates", durable: true, exclusive: false, autoDelete: false);

            var updateMessage = JsonSerializer.Serialize(new OrderUpdateMessage
            {
                OrderId = order.OrderId,
                Status = success ? OrderStatus.Completed : OrderStatus.Failed
            });

            var body = Encoding.UTF8.GetBytes(updateMessage);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "", routingKey: "order_updates", basicProperties: properties, body: body);

            Log.Information($"Order update sent: {order.OrderId} - Status: {success}");
        }
    }
}
