using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.Features.Orders.Commands.Checkout;
using Marketplace.Application.Features.Orders.Queries.GetMyOrders;
using Marketplace.Application.Features.Orders.Queries.GetOrderById;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/v1/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("checkout")]
    [ProducesResponseType(typeof(CheckoutResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Checkout(CheckoutCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);

        // ملاحظة: الدفع الفعلي (Payment Gateway) لسه مش هنا —
        // الخطوة المنطقية بعد كده: POST /payments/create-intent (راجعي phase3-customer-flow.md)
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetMyOrdersResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetMyOrdersQuery(page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id), ct);
        return Ok(result);
    }
}
