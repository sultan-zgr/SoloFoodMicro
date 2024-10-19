using RabbitMQ.Client;

namespace OrderService.Api.RabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private bool _disposed;

        public RabbitMQConnection()
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };
        }

        public bool IsConnected => _connection !=null && _connection.IsOpen &&!_disposed;

        public IModel CreateModel()
        {
            if(!IsConnected) TryConnect();
            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if(_disposed) return;
            _connection?.Dispose();
            _disposed = true;
        }

        public bool TryConnect()
        {
            _connection = _connectionFactory.CreateConnection();
            return IsConnected;
        }
    }
}
