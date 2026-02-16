using System.ComponentModel.DataAnnotations;
using MediatR;

namespace GoodStuff.CartApi.Application.Features.Commands.AddCart;

public class AddCartCommand : IRequest<Unit>
{
    [Required] public required string UserId { get; init; }
    [Required] public required string Id { get; init; }
    [Required] public required string Name { get; init; }
    [Required] public required int Quantity { get; init; }
    [Required] public required int Price { get; init; }
}
