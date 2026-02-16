using System.ComponentModel.DataAnnotations;
using MediatR;

namespace GoodStuff.CartApi.Application.Features.Commands.RemoveItem;

public class RemoveItemCommand : IRequest<Unit>
{
    [Required] public required string CartId { get; init; }
    [Required] public required string ProductId { get; init; }
}
