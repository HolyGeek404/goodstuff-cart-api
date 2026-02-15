using GoodStuff.CartApi.Application.Features.Queries.AddCartQuery;
using Microsoft.AspNetCore.Mvc;

namespace GoodStuff.CartApi.Presentation;

[Route("[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    [HttpPost]
    public IActionResult AddCart(AddCartQuery cart)
    {
        if (ModelState.IsValid)
            
            
        return Ok();
    }
    
    [HttpGet]
    public IActionResult GetCart([FromQuery] string  cartId)
    {
        return Ok();
    }
}