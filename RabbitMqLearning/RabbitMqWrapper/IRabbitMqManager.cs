namespace RabbitMqLearning.RabbitMqWrapper;

public interface IRabbitMqManager
{
    void CreateQueue(string message,string queueName,string exchangeName,string routingKey);
    string DeQueue(string queueName);
    void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey) where T : class;
    void PurgeQueue(string queueName);
    void CreateExchange(string exchangeName, string exchangeType);
}