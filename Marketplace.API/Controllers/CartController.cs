using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.Features.Cart.Commands.AddToCart;
using Marketplace.Application.Features.Cart.Commands.RemoveFromCart;
using Marketplace.Application.Features.Cart.Commands.UpdateCartItem;
using Marketplace.Application.Features.Cart.Queries.GetCart;

namespace Marketplace.API.Controllers;

// كل الـendpoints هنا بتشتغل على كارت المستخدم المسجل دخوله فقط —
// الـUserId بييجي من الـJWT جوه كل Handler، مفيش أي id بيتاخد من الـrequest
[ApiController]
[Route("api/v1/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCart(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCartQuery(), ct);
        return Ok(result);
    }

    [HttpPost("items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem(AddToCartCommand command, CancellationToken ct)
    {
        await _mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPut("items/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateItem(Guid productId, [FromBody] int quantity, CancellationToken ct)
    {
        await _mediator.Send(new UpdateCartItemCommand(productId, quantity), ct);
        return NoContent();
    }

    [HttpDelete("items/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveItem(Guid productId, CancellationToken ct)
    {
        await _mediator.Send(new RemoveFromCartCommand(productId), ct);
        return NoContent();
    }
}
