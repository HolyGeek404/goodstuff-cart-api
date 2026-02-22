using GoodStuff.CartApi.Application.DTO;
using GoodStuff.CartApi.Application.Services;
using MediatR;

namespace GoodStuff.CartApi.Application.Features.Queries.GetCart;

public class GetCartQueryHandler(ICacheService cacheService) : IRequestHandler<GetCartQuery, IReadOnlyCollection<ProductDto>>
{
    public async Task<IReadOnlyCollection<ProductDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        return await cacheService.GetCartAsync(request.UserId);
    }
}
