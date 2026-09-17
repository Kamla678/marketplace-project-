using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.Features.Sellers.Queries.GetSellerOrders;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/v1/seller/orders")]
[Authorize(Roles = "Seller")]
public class SellerOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SellerOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetSellerOrdersResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetSellerOrdersQuery(page, pageSize), ct);
        return Ok(result);
    }
}
