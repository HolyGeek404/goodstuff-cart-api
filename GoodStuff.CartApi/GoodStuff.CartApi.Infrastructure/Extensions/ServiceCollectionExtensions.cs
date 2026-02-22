using GoodStuff.CartApi.Application.Services;
using GoodStuff.CartApi.Infrastructure.Repositories;
using GoodStuff.CartApi.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace GoodStuff.CartApi.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNSTR") ??
                                        configuration.GetSection("Redis")["LocalConnStr"];

            return ConnectionMultiplexer.Connect(redisConnectionString!);
        });

        services.AddScoped<ISerializeService, SerializeService>();
        services.AddScoped<IRedisRepository, RedisRepository>();

        return services;
    }
}
