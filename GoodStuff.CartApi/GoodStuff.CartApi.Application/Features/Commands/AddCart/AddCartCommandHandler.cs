using MediatR;
using GoodStuff.CartApi.Application.Services;
using GoodStuff.CartApi.Domain;
using GoodStuff.CartApi.Domain.ValueObjects;

namespace GoodStuff.CartApi.Application.Features.Commands.AddCart;

public class AddCartCommandHandler(CacheService cacheService) : IRequestHandler<AddCartCommand, Unit>
{
    public async Task<Unit> Handle(AddCartCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = request.ProductId,
            Name = request.Name,
            Quantity = Quantity.Create(request.Quantity),
            Price = Price.Create(request.Price)
        };

        await cacheService.AddItemAsync(request.UserId, product);
        return Unit.Value;
    }
}
