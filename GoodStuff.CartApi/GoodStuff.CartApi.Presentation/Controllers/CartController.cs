using GoodStuff.CartApi.Application.Features.Commands.AddCart;
using GoodStuff.CartApi.Application.Features.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GoodStuff.CartApi.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
public class CartController(IMediator mediator, ILogger<CartController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddCart([FromBody] AddCartCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid cart");

            await mediator.Send(command);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed while adding item to cart {CartId}", command.CartId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while adding item to cart {CartId}", command.CartId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetCart([FromQuery] string cartId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(cartId))
                return BadRequest("cartId is required");

            var result = await mediator.Send(new GetCartQuery { CartId = cartId });
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed while retrieving cart {CartId}", cartId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while retrieving cart {CartId}", cartId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }
}
