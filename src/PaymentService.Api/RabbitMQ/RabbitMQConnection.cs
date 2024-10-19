using RabbitMQ.Client;

namespace PaymentService.Api.RabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactory _factory;
        private IConnection _connection;

        public RabbitMQConnection()
        {
            _factory = new ConnectionFactory() { HostName = "localhost" };
        }

        public IConnection GetConnection()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = _factory.CreateConnection();
            }
            return _connection;
        }
    }
}
