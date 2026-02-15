using GoodStuff.CartApi.Application.Features.Commands.AddCart;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GoodStuff.CartApi.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
public class CartController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddCart([FromBody] AddCartCommand command)
    {
        if (!ModelState.IsValid)
            return  BadRequest("Invalid cart");

        await mediator.Send(command);

        return Ok();
    }
    
    [HttpGet]
    public IActionResult GetCart([FromQuery] string  cartId)
    {
        return Ok();
    }
}
