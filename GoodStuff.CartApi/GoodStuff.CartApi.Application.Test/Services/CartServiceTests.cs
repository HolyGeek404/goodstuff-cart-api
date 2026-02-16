using GoodStuff.CartApi.Application.Services;
using GoodStuff.CartApi.Domain;
using GoodStuff.CartApi.Domain.ValueObjects;
using NSubstitute;
using StackExchange.Redis;

namespace GoodStuff.CartApi.Application.Test.Services;

[TestFixture]
public class CartServiceTests
{
    private IConnectionMultiplexer _connection = null!;
    private IDatabase _database = null!;
    private CartService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _connection = Substitute.For<IConnectionMultiplexer>();
        _database = Substitute.For<IDatabase>();
        _connection.GetDatabase(Arg.Any<int>(), Arg.Any<object?>()).Returns(_database);
        _sut = new CartService(_connection);
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Dispose();
    }

    [Test]
    public async Task AddItemAsync_ShouldIncrementHashAndSetExpiration()
    {
        const string userId = "user-1";
        var product = CreateProduct("product-1", 3);
        const string key = "cart:user:user-1";

        _database.HashIncrementAsync(key, product.Id, product.Quantity.Value, Arg.Any<CommandFlags>())
            .Returns(Task.FromResult((long)product.Quantity.Value));
        _database.KeyExpireAsync(key, TimeSpan.FromMinutes(60), Arg.Any<ExpireWhen>(), Arg.Any<CommandFlags>())
            .Returns(Task.FromResult(true));

        await _sut.AddItemAsync(userId, product);

        await _database.Received(1)
            .HashIncrementAsync(key, product.Id, product.Quantity.Value);
        await _database.Received(1)
            .KeyExpireAsync(key, TimeSpan.FromMinutes(60));
    }

    [Test]
    public async Task GetCartAsync_WhenNoEntries_ShouldReturnEmptyCollection()
    {
        _database.HashGetAllAsync("cart:user:user-1", Arg.Any<CommandFlags>())
            .Returns(Task.FromResult(Array.Empty<HashEntry>()));

        var result = await _sut.GetCartAsync("user-1");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetCartAsync_WhenEntriesExist_ShouldMapEntriesToProducts()
    {
        var entries = new[]
        {
            new HashEntry("p1", 2),
            new HashEntry("p2", 5)
        };

        _database.HashGetAllAsync("cart:user:user-1", Arg.Any<CommandFlags>())
            .Returns(Task.FromResult(entries));

        var result = await _sut.GetCartAsync("user-1");

        var products = result.ToList();
        Assert.That(products, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(products[0].Id, Is.EqualTo("p1"));
            Assert.That(products[0].Name, Is.EqualTo("p1"));
            Assert.That(products[0].Quantity.Value, Is.EqualTo(2));
            Assert.That(products[0].Price.Value, Is.EqualTo(0));
            Assert.That(products[1].Id, Is.EqualTo("p2"));
            Assert.That(products[1].Name, Is.EqualTo("p2"));
            Assert.That(products[1].Quantity.Value, Is.EqualTo(5));
            Assert.That(products[1].Price.Value, Is.EqualTo(0));
        }
    }

    [Test]
    public async Task RemoveItemAsync_WhenItemWasNotRemoved_ShouldNotDoAnythingElse()
    {
        const string key = "cart:user:user-1";

        _database.HashDeleteAsync(key, "product-1", Arg.Any<CommandFlags>())
            .Returns(Task.FromResult(false));

        await _sut.RemoveItemAsync("user-1", "product-1");

        await _database.Received(1).HashDeleteAsync(key, "product-1");
        await _database.DidNotReceive().HashLengthAsync(Arg.Any<RedisKey>(), Arg.Any<CommandFlags>());
        await _database.DidNotReceive().KeyDeleteAsync(Arg.Any<RedisKey>(), Arg.Any<CommandFlags>());
        await _database.DidNotReceive().KeyExpireAsync(
            Arg.Any<RedisKey>(),
            Arg.Any<TimeSpan?>(),
            Arg.Any<ExpireWhen>(),
            Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task RemoveItemAsync_WhenCartBecomesEmpty_ShouldDeleteKey()
    {
        const string key = "cart:user:user-1";

        _database.HashDeleteAsync(key, "product-1", Arg.Any<CommandFlags>())
            .Returns(Task.FromResult(true));
        _database.HashLengthAsync(key, Arg.Any<CommandFlags>())
            .Returns(Task.FromResult(0L));
        _database.KeyDeleteAsync(key, Arg.Any<CommandFlags>())
            .Returns(Task.FromResult(true));

        await _sut.RemoveItemAsync("user-1", "product-1");

        await _database.Received(1).HashDeleteAsync(key, "product-1");
        await _database.Received(1).HashLengthAsync(key);
        await _database.Received(1).KeyDeleteAsync(key);
        await _database.DidNotReceive().KeyExpireAsync(
            Arg.Any<RedisKey>(),
            Arg.Any<TimeSpan?>(),
            Arg.Any<ExpireWhen>(),
            Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task RemoveItemAsync_WhenCartStillHasItems_ShouldRefreshExpiration()
    {
        const string key = "cart:user:user-1";

        _database.HashDeleteAsync(key, "product-1", Arg.Any<CommandFlags>())
            .Returns(Task.FromResult(true));
        _database.HashLengthAsync(key, Arg.Any<CommandFlags>())
            .Returns(Task.FromResult(2L));
        _database.KeyExpireAsync(key, TimeSpan.FromMinutes(60), Arg.Any<ExpireWhen>(), Arg.Any<CommandFlags>())
            .Returns(Task.FromResult(true));

        await _sut.RemoveItemAsync("user-1", "product-1");

        await _database.Received(1).HashDeleteAsync(key, "product-1");
        await _database.Received(1).HashLengthAsync(key);
        await _database.Received(1)
            .KeyExpireAsync(key, TimeSpan.FromMinutes(60));
        await _database.DidNotReceive().KeyDeleteAsync(Arg.Any<RedisKey>(), Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task ClearCartAsync_ShouldDeleteCartKey()
    {
        const string key = "cart:user:user-1";

        _database.KeyDeleteAsync(key, Arg.Any<CommandFlags>())
            .Returns(Task.FromResult(true));

        await _sut.ClearCartAsync("user-1");

        await _database.Received(1).KeyDeleteAsync(key);
    }

    private static Product CreateProduct(string id, int quantity)
    {
        return new Product
        {
            Id = id,
            Name = id,
            Quantity = Quantity.Create(quantity),
            Price = Price.Create(10)
        };
    }
}
