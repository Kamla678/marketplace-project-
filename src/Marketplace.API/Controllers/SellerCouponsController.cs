using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.Features.Coupons.Commands.CreateCoupon;
using Marketplace.Application.Features.Coupons.Queries.GetMyCoupons;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/v1/seller/coupons")]
[Authorize(Roles = "Seller")]
public class SellerCouponsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SellerCouponsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CouponDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyCoupons(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyCouponsQuery(), ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(CreateCouponCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, new { id });
    }
}
