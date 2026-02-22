using System.Text.Json;
using GoodStuff.CartApi.Application.DTO;
using GoodStuff.CartApi.Domain;
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
        var productJson = JsonSerializer.Serialize(new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price.Value,
            Quantity = product.Quantity.Value,
        });
        
        await _redis.HashSetAsync(key, product.Id, productJson);
        await _redis.KeyExpireAsync(key, TimeSpan.FromMinutes(CartExpirationMinutes));
    }

    public async Task<IReadOnlyCollection<ProductDto>> GetCartAsync(string userId)
    {
        var key = GetCartKey(userId);

        var entries = await _redis.HashGetAllAsync(key);
        return entries.Select(e => JsonSerializer.Deserialize<ProductDto>(e.Value.ToString())).ToList()!;
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