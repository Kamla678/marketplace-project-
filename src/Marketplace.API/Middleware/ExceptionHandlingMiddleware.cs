using System.Net;
using System.Text.Json;
using FluentValidation;
using Marketplace.Domain.Exceptions;
using StripeException = Stripe.StripeException;

namespace Marketplace.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                "Validation failed",
                validationEx.Errors.Select(e => e.ErrorMessage).ToArray()
            ),
            EmailAlreadyExistsException => (HttpStatusCode.Conflict, exception.Message, Array.Empty<string>()),
            SkuAlreadyExistsException => (HttpStatusCode.Conflict, exception.Message, Array.Empty<string>()),
            InvalidCredentialsException => (HttpStatusCode.Unauthorized, exception.Message, Array.Empty<string>()),
            InvalidRefreshTokenException => (HttpStatusCode.Unauthorized, exception.Message, Array.Empty<string>()),
            ProductNotFoundException => (HttpStatusCode.NotFound, exception.Message, Array.Empty<string>()),
            CartEmptyException => (HttpStatusCode.BadRequest, exception.Message, Array.Empty<string>()),
            InsufficientStockException => (HttpStatusCode.Conflict, exception.Message, Array.Empty<string>()),
            InvalidCouponException => (HttpStatusCode.BadRequest, exception.Message, Array.Empty<string>()),
            DuplicateReviewException => (HttpStatusCode.Conflict, exception.Message, Array.Empty<string>()),
            OrderNotPayableException => (HttpStatusCode.Conflict, exception.Message, Array.Empty<string>()),
            StripeException => (HttpStatusCode.BadRequest, "Invalid webhook signature or payload.", Array.Empty<string>()),
            CategorySlugExistsException => (HttpStatusCode.Conflict, exception.Message, Array.Empty<string>()),
            CouponCodeExistsException => (HttpStatusCode.Conflict, exception.Message, Array.Empty<string>()),
            // لازم يكون آخر case في السلسلة عشان الـexceptions المتخصصة فوق تتلقط الأول
            DomainException => (HttpStatusCode.BadRequest, exception.Message, Array.Empty<string>()),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred", Array.Empty<string>())
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception occurred");

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            statusCode = (int)statusCode,
            message,
            errors,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
