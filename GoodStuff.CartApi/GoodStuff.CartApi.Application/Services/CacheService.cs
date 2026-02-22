using GoodStuff.CartApi.Application.DTO;
using GoodStuff.CartApi.Domain;

namespace GoodStuff.CartApi.Application.Services;

public class CacheService(IRedisRepository redisRepository)
{
    public async Task AddItemAsync(string userId, Product product) => await redisRepository.AddCartItem(userId, ProductMapper.Map(product));
    public async Task<IReadOnlyList<ProductDto?>> GetCartAsync(string userId) => await redisRepository.GetCartAsync(userId);
    public async Task<bool> RemoveItemAsync(string userId, string productId) => await redisRepository.RemoveItemAsync(userId, productId);
    public async Task ClearCartAsync(string userId) => await redisRepository.ClearCartAsync(userId);
}