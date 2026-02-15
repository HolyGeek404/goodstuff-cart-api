namespace GoodStuff.CartApi.Domain.ValueObjects;

public record Product()
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required int Quantity { get; init; }
}