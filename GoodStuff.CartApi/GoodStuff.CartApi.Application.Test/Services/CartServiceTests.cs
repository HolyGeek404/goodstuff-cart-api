using GoodStuff.CartApi.Application.Services;
using GoodStuff.CartApi.Domain;
using GoodStuff.CartApi.Domain.ValueObjects;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace GoodStuff.CartApi.Application.Test.Services;

public class CartServiceTests
{
    private readonly Mock<IDatabase> _databaseMock;
    private readonly CartService cartService;

    public CartServiceTests()
    {
        _databaseMock = new Mock<IDatabase>();
        var connMock = new Mock<IConnectionMultiplexer>();
        cartService = new CartService(connMock.Object);
    }
}