using MediatR;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.Features.Products.Queries.GetProductById;
using Marketplace.Application.Features.Products.Queries.GetProducts;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>قائمة منتجات معتمدة ومتاحة، مع فلاتر و Pagination</summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetProductsResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] Guid? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetProductsQuery(categoryId, minPrice, maxPrice, q, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), ct);
        return Ok(result);
    }
}
