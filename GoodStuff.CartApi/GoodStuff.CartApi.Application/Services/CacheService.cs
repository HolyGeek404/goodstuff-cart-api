using GoodStuff.CartApi.Domain;
using GoodStuff.CartApi.Domain.ValueObjects;
using StackExchange.Redis;

namespace GoodStuff.CartApi.Application.Services;

public class CartService(IConnectionMultiplexer connection)
{
    private readonly IDatabase _redis = connection.GetDatabase();
    private const int CartExpirationMinutes = 60;

    private static string GetCartKey(string userId) => $"cart:user:{userId}";

    public async Task AddItemAsync(string userId, Product product)
    {
        var key = GetCartKey(userId);

        await _redis.HashIncrementAsync(key, product.Id, product.Quantity.Value);
        await _redis.KeyExpireAsync(key, TimeSpan.FromMinutes(CartExpirationMinutes));
    }

    public async Task<IReadOnlyCollection<Product>> GetCartAsync(string userId)
    {
        var key = GetCartKey(userId);

        var entries = await _redis.HashGetAllAsync(key);

        if (entries.Length == 0)
            return [];

        var items = entries
            .Select(entry => new Product
            {
                Id = entry.Name.ToString(),
                Name = entry.Name.ToString(),
                Quantity = Quantity.Create((int)entry.Value),
                Price = Price.Create(0)
            }).ToList();

        return items;
    }

    public async Task RemoveItemAsync(string userId, string productId)
    {
        var key = GetCartKey(userId);

        var removed = await _redis.HashDeleteAsync(key, productId);
        if (!removed)
            return;

        var remainingItems = await _redis.HashLengthAsync(key);
        if (remainingItems == 0)
        {
            await _redis.KeyDeleteAsync(key);
            return;
        }

        await _redis.KeyExpireAsync(key, TimeSpan.FromMinutes(CartExpirationMinutes));
    }

    public async Task ClearCartAsync(string userId)
    {
        var key = GetCartKey(userId);

        await _redis.KeyDeleteAsync(key);
    }
}
