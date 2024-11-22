using System.Text;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqLearning.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class RabbitMqController : ControllerBase
{
    [HttpPost]
    public IActionResult JustTest([FromForm] string message)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare
        (
            queue: "MyQueues",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish
        (
            exchange: "",
            routingKey: "MyQueues",
            basicProperties: null,
            body: body
        );

        return Ok();
    }

    [HttpPut]
    public IActionResult DeQueue()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };
        string message = "";
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            
            channel.QueueDeclare
            (
                queue: "MyQueues",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            channel.BasicConsume("MyQueues", false, consumer);
            consumer.Received += (model, ea) =>
            {
                 message = Encoding.UTF8.GetString(ea.Body.ToArray());
               // channel.BasicAck(ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: "MyQueues",
                autoAck: false,
                consumer: consumer);
        }

        return Ok(message);
    }
}