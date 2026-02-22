using System.Text.Json;
using GoodStuff.CartApi.Application.DTO;
using GoodStuff.CartApi.Domain;
using StackExchange.Redis;

namespace GoodStuff.CartApi.Application.Services;

public class CacheService(IConnectionMultiplexer connection)
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
        

    }

    public async Task<IReadOnlyCollection<ProductDto>> GetCartAsync(string userId)
    {
        var key = GetCartKey(userId);

        var entries = await _redis.HashGetAllAsync(key);
        return entries.Select(e => JsonSerializer.Deserialize<ProductDto>(e.Value.ToString())).ToList()!;
    }

    public async Task<bool> RemoveItemAsync(string userId, string productId)
    {
        var key = GetCartKey(userId);

        var removed = await _redis.HashDeleteAsync(key, productId);
        return removed;
    }

    public async Task ClearCartAsync(string userId)
    {
        var key = GetCartKey(userId);

        await _redis.KeyDeleteAsync(key);
    }
}