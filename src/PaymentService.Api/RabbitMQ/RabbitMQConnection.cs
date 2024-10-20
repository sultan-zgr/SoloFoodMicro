using RabbitMQ.Client;
using Serilog;

namespace PaymentService.Api.RabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection, IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private bool _disposed;

        public RabbitMQConnection(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                TryConnect();
            }
            return _connection.CreateModel();
        }

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

        public void Dispose()
        {
            if (_disposed) return;
            _connection?.Close();
            _connection?.Dispose();
            _disposed = true;
        }
    }
}
