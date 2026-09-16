using MediatR;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Application.Features.Auth.Commands.Login;
using Marketplace.Application.Features.Auth.Commands.RefreshToken;
using Marketplace.Application.Features.Auth.Commands.Register;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);

        // الـRefresh Token يترجع كـHttpOnly Cookie — مش JS-accessible، بيقلل خطر XSS
        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(new LoginResponse(result.AccessToken, result.AccessTokenExpiresAt, result.UserName, result.Role));
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(RefreshResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new { message = "Refresh token not found" });

        var result = await _mediator.Send(new RefreshTokenCommand(request.UserId, refreshToken), ct);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(new RefreshResponse(result.AccessToken, result.AccessTokenExpiresAt));
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("refreshToken");
        return NoContent();
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(14)
        });
    }
}

public record LoginResponse(string AccessToken, DateTime AccessTokenExpiresAt, string UserName, string Role);
public record RefreshResponse(string AccessToken, DateTime AccessTokenExpiresAt);
public record RefreshTokenRequest(Guid UserId);
