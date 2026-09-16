using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.Features.Products.Commands.ApproveProduct;
using Marketplace.Application.Features.Products.Commands.RejectProduct;
using Marketplace.Application.Features.Products.Queries.GetPendingProducts;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/v1/admin/products")]
[Authorize(Roles = "Admin")]
public class AdminProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("pending")]
    [ProducesResponseType(typeof(GetPendingProductsResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPending([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPendingProductsQuery(page, pageSize), ct);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new ApproveProductCommand(id), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(Guid id, RejectProductRequest request, CancellationToken ct)
    {
        await _mediator.Send(new RejectProductCommand(id, request.Reason), ct);
        return NoContent();
    }
}

public record RejectProductRequest(string Reason);
