using RabbitMQ.Client;

namespace OrderService.Api.RabbitMQ
{
    public interface IRabbitMQConnection : IDisposable
    {
        IModel CreateModel();
        bool IsConnected { get; }
        IConnection GetConnection();
        bool TryConnect();
    }
}
