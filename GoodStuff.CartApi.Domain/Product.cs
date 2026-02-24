using GoodStuff.CartApi.Domain.ValueObjects;

namespace GoodStuff.CartApi.Domain;

public record Product()
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required Quantity Quantity { get; init; }
    public required Price Price { get; init; }
}