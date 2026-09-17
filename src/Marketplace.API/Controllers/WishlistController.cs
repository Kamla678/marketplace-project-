using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.Features.Wishlist.Commands.AddToWishlist;
using Marketplace.Application.Features.Wishlist.Commands.RemoveFromWishlist;
using Marketplace.Application.Features.Wishlist.Queries.GetWishlist;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/v1/wishlist")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IMediator _mediator;

    public WishlistController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<WishlistItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetWishlistQuery(), ct);
        return Ok(result);
    }

    [HttpPost("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Add(Guid productId, CancellationToken ct)
    {
        await _mediator.Send(new AddToWishlistCommand(productId), ct);
        return NoContent();
    }

    [HttpDelete("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remove(Guid productId, CancellationToken ct)
    {
        await _mediator.Send(new RemoveFromWishlistCommand(productId), ct);
        return NoContent();
    }
}
