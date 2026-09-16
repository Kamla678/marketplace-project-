using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.Features.Reviews.Commands.AddReview;
using Marketplace.Application.Features.Reviews.Queries.GetProductReviews;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/v1")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // عام — أي حد يقدر يشوف التقييمات من غير تسجيل دخول
    [HttpGet("products/{productId:guid}/reviews")]
    [ProducesResponseType(typeof(GetProductReviewsResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductReviews(Guid productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetProductReviewsQuery(productId, page, pageSize), ct);
        return Ok(result);
    }

    // محتاج تسجيل دخول — الـUserId بييجي من التوكن مش من الـbody
    [HttpPost("products/{productId:guid}/reviews")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddReview(Guid productId, AddReviewRequest request, CancellationToken ct)
    {
        var reviewId = await _mediator.Send(new AddReviewCommand(productId, request.Rating, request.Comment), ct);
        return StatusCode(StatusCodes.Status201Created, new { reviewId });
    }
}

public record AddReviewRequest(int Rating, string? Comment);
