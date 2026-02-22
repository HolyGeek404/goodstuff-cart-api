using GoodStuff.CartApi.Application.DTO;
using GoodStuff.CartApi.Domain;

namespace GoodStuff.CartApi.Application.Services;

public interface ICacheService
{
    Task AddItemAsync(string userId, Product product);
    Task<IReadOnlyList<ProductDto>> GetCartAsync(string userId);
    Task<bool> RemoveItemAsync(string userId, string productId);
    Task ClearCartAsync(string userId);
}
