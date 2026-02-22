using System.Text.Json;
using GoodStuff.CartApi.Application.DTO;
using GoodStuff.CartApi.Application.Services;
using GoodStuff.CartApi.Domain;
using GoodStuff.CartApi.Domain.ValueObjects;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace GoodStuff.CartApi.Application.Test.Services;

public class CacheServiceTests
{
    private readonly Mock<IDatabase> _databaseMock;
    private readonly CacheService _cacheService;

    public CacheServiceTests()
    {
        _databaseMock = new Mock<IDatabase>();
        var connMock = new Mock<IConnectionMultiplexer>();
        connMock.Setup(x => x.GetDatabase(It.IsAny<int>())).Returns(_databaseMock.Object);
        _cacheService = new CacheService(connMock.Object);
    }

    [Fact]
    public async Task AddItemAsync_AddItem_Success()
    {
        // Arrange
        const string userId = "test";
        const string key = $"cart:user:{userId}";
        const int cartExpirationMinutes = 60;
        var product = new Product
        {
            Id = "432",
            Name = "GPU",
            Price = Price.Create(32),
            Quantity = Quantity.Create(3)
        };
        var productJson = JsonSerializer.Serialize(new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price.Value,
            Quantity = product.Quantity.Value,
        });

        
        // Act
        await _cacheService.AddItemAsync(userId, product);
        
        // Assert
        _databaseMock.Verify(x => x.HashSetAsync(key, product.Id, productJson), Times.Once);
        _databaseMock.Verify(x => x.KeyExpireAsync(key, TimeSpan.FromMinutes(cartExpirationMinutes)), Times.Once);
    }
}