using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.Features.Products.Commands.CreateProduct;
using Marketplace.Application.Features.Products.Commands.UpdatePrice;
using Marketplace.Application.Features.Products.Commands.UpdateStock;
using Marketplace.Application.Features.Products.Queries.GetMyProducts;

namespace Marketplace.API.Controllers;

// كل الـendpoints هنا بترجع/تعدل بس منتجات الـSeller المسجل دخوله —
// الـSellerId مستخرج من الـJWT جوه كل Handler، مش من الـURL ولا الـbody
[ApiController]
[Route("api/v1/seller/products")]
[Authorize(Roles = "Seller")]
public class SellerProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SellerProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetMyProductsResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetMyProductsQuery(page, pageSize), ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateProductResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateProductCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPatch("{id:guid}/stock")]
    [ProducesResponseType(typeof(UpdateStockResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStock(Guid id, UpdateStockRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateStockCommand(id, request.Quantity, request.Operation), ct);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/price")]
    [ProducesResponseType(typeof(UpdatePriceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePrice(Guid id, UpdatePriceRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdatePriceCommand(id, request.NewPrice, request.DiscountPrice), ct);
        return Ok(result);
    }
}

public record UpdateStockRequest(int Quantity, StockOperation Operation);
public record UpdatePriceRequest(decimal NewPrice, decimal? DiscountPrice);
