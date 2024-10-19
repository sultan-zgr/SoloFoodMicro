using RabbitMQ.Client;

namespace PaymentService.Api.RabbitMQ
{
    public interface IRabbitMQConnection
    {
        IConnection GetConnection();
    }
}
