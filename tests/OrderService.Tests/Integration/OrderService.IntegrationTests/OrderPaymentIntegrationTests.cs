using OrderService.Api.RabbitMQ;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.IntegrationTests
{
    public class OrderPaymentIntegrationTests
    {
        private readonly IRabbitMQConnection _rabbitMQConnection;

        public OrderPaymentIntegrationTests()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _rabbitMQConnection = new RabbitMQConnection(factory);
            _rabbitMQConnection.TryConnect(); // Bağlantıyı kontrol et ve kur

        }
    }
}
