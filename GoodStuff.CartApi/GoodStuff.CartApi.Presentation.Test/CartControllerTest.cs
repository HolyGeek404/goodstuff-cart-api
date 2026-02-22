using System.Net.Http.Json;
using System.Text.Json;
using GoodStuff.CartApi.Application.Features.Commands.AddCart;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GoodStuff.CartApi.Presentation.Test;

public class CartControllerTest(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly AddCartCommand _addCartCommand = new()
    {
        UserId = "test",
        ProductId = "543",
        Name = "GPU",
        Quantity = 6,
        Price = 1200
    };

    [Fact]
    public async Task AddCart_InsertCartToCache_ShouldReturnOk()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/Cart",_addCartCommand);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetCart_WhenCartHasItems_ShouldReturnItems()
    {
        await _client.PostAsJsonAsync("/Cart", _addCartCommand);

        var response = await _client.GetAsync($"/Cart?userId={_addCartCommand.UserId}");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(content);
        var items = json.RootElement;

        Assert.Equal(JsonValueKind.Array, items.ValueKind);
        Assert.True(items.GetArrayLength() > 0);
        var firstItem = items[0];
        Assert.Equal(_addCartCommand.ProductId, firstItem.GetProperty("id").GetString());
        Assert.Equal(_addCartCommand.Name, firstItem.GetProperty("name").GetString());
        Assert.Equal(_addCartCommand.Price, firstItem.GetProperty("price").GetInt32());
    }

    [Fact]
    public async Task RemoveItem_WhenItemExists_ShouldReturnOkAndRemoveFromCart()
    {
        var userId = $"test-{Guid.NewGuid():N}";
        var command = new AddCartCommand
        {
            UserId = userId,
            ProductId = "prod-1",
            Name = "GPU",
            Quantity = 1,
            Price = 1200
        };

        await _client.PostAsJsonAsync("/Cart", command);

        var removeResponse = await _client.DeleteAsync($"/Cart/{userId}/items/{command.ProductId}");
        removeResponse.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync($"/Cart?userId={userId}");
        getResponse.EnsureSuccessStatusCode();
        var content = await getResponse.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(content);
        var items = json.RootElement;

        Assert.Equal(JsonValueKind.Array, items.ValueKind);
        Assert.Equal(0, items.GetArrayLength());
    }
}
