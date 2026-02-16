using GoodStuff.CartApi.Application.Services;
using GoodStuff.CartApi.Domain;
using GoodStuff.CartApi.Domain.ValueObjects;
using MediatR;

namespace GoodStuff.CartApi.Application.Features.Queries.GetCart;

public class GetCartQueryHandler(CartService cartService) : IRequestHandler<GetCartQuery, IReadOnlyCollection<Product>>
{
    public async Task<IReadOnlyCollection<Product>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
       return await cartService.GetCartAsync(request.UserId);
    }
}
