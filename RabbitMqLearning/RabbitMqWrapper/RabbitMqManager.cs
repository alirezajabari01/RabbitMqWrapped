using System.Text;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqLearning.RabbitMq;

namespace RabbitMqLearning.RabbitMqWrapper;

public class RabbitMqManager : IRabbitMqManager
{
    private readonly DefaultObjectPool<IModel> _objectPool;
    private readonly IModel _channel;

    public RabbitMqManager(IPooledObjectPolicy<IModel> objectPolicy)
    {
        _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
    }

    public void CreateQueue(string message, string queueName, string exchangeName, string routingKey)
    {
        _channel.QueueDeclare
        (
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.QueueBind(queueName, exchangeName, routingKey);

        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish
        (
            exchange: exchangeName,
            routingKey: routingKey,
            basicProperties: null,
            body: body
        );
    }

    public string DeQueue(string queueName)
    {
        var consumer = new EventingBasicConsumer(_channel);

        var res = _channel.BasicGet(queueName, true);

        // _channel.BasicAck(deliveryTag:1,false);

        return Encoding.UTF8.GetString(res.Body.ToArray());
    }

    public void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey) where T : class
    {
        if (message.Equals(null)) return;

        var channel = _objectPool.Get();
        try
        {
            //channel.ExchangeDeclare(exchangeName, exchangeType, true, false, null);
            var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(exchangeName, routeKey, properties, sendBytes);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            _objectPool.Return(channel);
        }
    }

    public void PurgeQueue(string queueName)
    {
        _channel.QueuePurge(queueName);
    }

    public void CreateExchange(string exchangeName, string exchangeType)
    {
        //ExchangeType t = (ExchangeType)Enum.Parse(typeof(ExchangeType), exchangeType);
        var channel = _objectPool.Get();
        channel.ExchangeDeclare
        (
            exchangeName,
            exchangeType,
            true,
            false
        );
    }
}