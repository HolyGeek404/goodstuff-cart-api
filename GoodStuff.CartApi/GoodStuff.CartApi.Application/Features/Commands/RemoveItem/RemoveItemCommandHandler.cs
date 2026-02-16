using GoodStuff.CartApi.Application.Services;
using MediatR;

namespace GoodStuff.CartApi.Application.Features.Commands.RemoveItem;

public class RemoveItemCommandHandler(CartService cartService) : IRequestHandler<RemoveItemCommand, Unit>
{
    public async Task<Unit> Handle(RemoveItemCommand request, CancellationToken cancellationToken)
    {
        await cartService.RemoveItemAsync(request.CartId, request.ProductId);
        return Unit.Value;
    }
}
