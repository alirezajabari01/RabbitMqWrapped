using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMqLearning.RabbitMqWrapper;

namespace RabbitMqLearning.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class RabbitMqWrapperController : ControllerBase
{
    private readonly IRabbitMqManager _rabbitMqManager;

    public RabbitMqWrapperController(IRabbitMqManager rabbitMqManager)
    {
        _rabbitMqManager = rabbitMqManager;
    }

    [HttpPost]
    public void DeclareExchange(string exchangeName,string exchangeType)
    {
        _rabbitMqManager.CreateExchange(exchangeName,exchangeType);
    }
    
    [HttpDelete]
    public IActionResult PurgeQueue(string queueName)
    {
        _rabbitMqManager.PurgeQueue(queueName);
        return Ok();
    }

    [HttpPost]
    public IActionResult Queue([FromForm] string message, string queueName, string exchangeName, string routingKey)
    {
        _rabbitMqManager.CreateQueue(message, queueName, exchangeName, routingKey);
        return Ok();
    }

    [HttpGet]
    public IActionResult DeQueue(string queueName)
    {
        string result = _rabbitMqManager.DeQueue(queueName);
        return Ok(result);
    }

    [HttpPost]
    public IActionResult Publish([FromForm] string message, string exchangeType, string exchangeName, string routingKey)
    {
        _rabbitMqManager.Publish(message, exchangeName, exchangeType, routingKey);
        return Ok();
    }
}