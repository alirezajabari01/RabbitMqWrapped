using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using RabbitMqLearning.RabbitMq;

namespace RabbitMqLearning;

public static class RabbitServiceCollectionExtensions
{
    public static IServiceCollection AddRabbit(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitConfig = configuration.GetSection("rabbit");
        services.Configure<RabbitOptions>(rabbitConfig);
        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitModelPooledObjectPolicy>();
        //services.AddSingleton<IRabbitManager, RabbitManager>();
        return services;
    } 
}