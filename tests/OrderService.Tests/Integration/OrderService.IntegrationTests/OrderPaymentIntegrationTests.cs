using Microsoft.Extensions.DependencyInjection;
using OrderService.Api.Models;
using OrderService.Api.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Xunit;
using FluentAssertions;
using OrderService.Api.DTOs;
using StackExchange.Redis;

namespace OrderService.IntegrationTests
{
    public class OrderPaymentIntegrationTests
    {
        private readonly IRabbitMQConnection _rabbitMQConnection;
        private readonly IDatabase _redis;

        public OrderPaymentIntegrationTests()
        {
            // RabbitMQ bağlantısını oluşturmak için ConnectionFactory kullan
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _rabbitMQConnection = new RabbitMQConnection(factory); // Hata burada düzeltildi
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _redis = redis.GetDatabase();
        }

        [Fact]
        public void OrderMessage_ShouldBeConsumedByPaymentService()
        {
            // Arrange: RabbitMQ üzerinden gönderilecek siparişi hazırla
            var orderMessage = new OrderMessage
            {
                OrderId = 1,
                CustomerName = "Ali Veli",
                Product = "Laptop",
                Price = 15000.50M
            };

            using var channel = _rabbitMQConnection.CreateModel();
            channel.QueueDeclare(queue: "orders", durable: false, exclusive: false, autoDelete: false);

            var message = JsonSerializer.Serialize(orderMessage);
            var body = Encoding.UTF8.GetBytes(message);

            // Act: Sipariş mesajını RabbitMQ'ya gönder
            channel.BasicPublish(exchange: "", routingKey: "orders", body: body);

            // Assert: PaymentService'in mesajı aldığını doğrula
            var consumer = new EventingBasicConsumer(channel);
            bool messageReceived = false;

            consumer.Received += (model, ea) =>
            {
                var receivedMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
                var receivedOrder = JsonSerializer.Deserialize<OrderMessage>(receivedMessage);

                receivedOrder.Should().NotBeNull();
                receivedOrder.OrderId.Should().Be(orderMessage.OrderId);
                messageReceived = true;
            };

            channel.BasicConsume(queue: "orders", autoAck: true, consumer: consumer);

            // 5 saniye bekle ve mesajın tüketildiğini doğrula
            Task.Delay(5000).Wait();
            messageReceived.Should().BeTrue();
        }
        [Fact]
        public async Task OrderMessage_ShouldBeProcessedAndStoredInRedis()
        {
            // Arrange: OrderMessage nesnesi oluştur
            var orderMessage = new OrderMessage
            {
                OrderId = 1,
                CustomerName = "Ali Veli",
                Product = "Laptop",
                Price = 15000.50M
            };

            // RabbitMQ'ya sipariş mesajını gönder
            PublishOrderMessage(orderMessage);

            // 5 saniye bekle (PaymentConsumer'ın mesajı işlemesi için)
            await Task.Delay(5000);

            // Redis'ten veriyi kontrol et
            var redisData = await _redis.StringGetAsync($"Order:{orderMessage.OrderId}");

            // Assert: Redis'teki verinin var olup olmadığını doğrula
            redisData.HasValue.Should().BeTrue("Order should be stored in Redis.");

            var storedOrder = JsonSerializer.Deserialize<OrderMessage>(redisData);
            storedOrder.Should().NotBeNull();
            storedOrder.OrderId.Should().Be(orderMessage.OrderId);
        }

        private void PublishOrderMessage(OrderMessage orderMessage)
        {
            using var channel = _rabbitMQConnection.GetConnection().CreateModel();
            channel.QueueDeclare(queue: "orders", durable: false, exclusive: false, autoDelete: false);

            var message = JsonSerializer.Serialize(orderMessage);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "orders", body: body);

            // Log: Sipariş mesajının gönderildiğini doğrula
            System.Console.WriteLine($"Order message sent: {orderMessage.OrderId}");
        }
    }

}

