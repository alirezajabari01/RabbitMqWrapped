using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace RabbitMqLearning.RabbitMq;

public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>
{
    private readonly RabbitOptions _rabbitOptions;
    private readonly IConnection _connection;

    public RabbitModelPooledObjectPolicy(IOptions<RabbitOptions> optionsAccess)
    {
        _rabbitOptions = optionsAccess.Value;
        _connection = GetConnection();
    }

    private IConnection GetConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitOptions.HostName,
        };
        return factory.CreateConnection();
    }

    public IModel Create()
    {
        return _connection.CreateModel();
    }

    public bool Return(IModel obj)
    {
        if (obj.IsOpen) return true;
        obj?.Dispose();
        return false;

    }
}