using RabbitMQ.Client;
using Serilog;

namespace OrderService.Api.RabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private bool _disposed;

        public RabbitMQConnection(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public bool TryConnect()
        {
            try
            {
                _connection = _connectionFactory.CreateConnection();
                Log.Information("RabbitMQ connected.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"RabbitMQ connection failed: {ex.Message}");
                return false;
            }
        }

        public IConnection GetConnection()
        {
            if (!IsConnected)
            {
                TryConnect();
            }
            return _connection;
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available.");
            }
            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _connection?.Close();
            _connection?.Dispose();
            _disposed = true;
        }
    }
}
