using GoodStuff.CartApi.Application.DTO;
using MediatR;

namespace GoodStuff.CartApi.Application.Features.Queries.GetCart;

public class GetCartQuery : IRequest<IReadOnlyCollection<ProductDto>>
{
    public required string UserId { get; init; }
}
