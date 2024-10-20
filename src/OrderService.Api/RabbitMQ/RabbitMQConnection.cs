using RabbitMQ.Client;
using System;

namespace OrderService.Api.RabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection, IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private bool _disposed;

        public RabbitMQConnection(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _connection?.Close();
            _connection?.Dispose();
        }

        public bool TryConnect()
        {
            _connection = _connectionFactory.CreateConnection();
            return IsConnected;
        }
    }
}
