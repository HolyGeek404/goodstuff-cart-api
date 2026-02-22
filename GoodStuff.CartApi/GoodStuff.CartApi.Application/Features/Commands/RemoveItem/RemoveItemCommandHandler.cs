using GoodStuff.CartApi.Application.Services;
using MediatR;

namespace GoodStuff.CartApi.Application.Features.Commands.RemoveItem;

public class RemoveItemCommandHandler(CacheService cacheService) : IRequestHandler<RemoveItemCommand, Unit>
{
    public async Task<Unit> Handle(RemoveItemCommand request, CancellationToken cancellationToken)
    {
        await cacheService.RemoveItemAsync(request.UserId, request.ProductId);
        return Unit.Value;
    }
}