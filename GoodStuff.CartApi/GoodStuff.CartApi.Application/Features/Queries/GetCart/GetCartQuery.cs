using GoodStuff.CartApi.Domain;
using MediatR;

namespace GoodStuff.CartApi.Application.Features.Queries.GetCart;

public class GetCartQuery : IRequest<IReadOnlyCollection<Product>>
{
    public required string UserId { get; init; }
}
