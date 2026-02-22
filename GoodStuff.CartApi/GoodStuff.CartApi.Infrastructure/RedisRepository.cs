using StackExchange.Redis;
using static System.Int32;

namespace GoodStuff.CartApi.Infrastructure;

public class RedisRepository(IConnectionMultiplexer connection)
{
    private readonly IDatabase _redis = connection.GetDatabase();
    private static int CartExpirationMinutes() => TryParse(Environment.GetEnvironmentVariable("REDIS_CONNSTR"), out var expirationTime) ? expirationTime : 60;

    public async Task AddCartItem(string key, string productId, string product)
    {
        await _redis.HashSetAsync(key, productId, product);
        await _redis.KeyExpireAsync(key, TimeSpan.FromMinutes(CartExpirationMinutes()));
    }

}