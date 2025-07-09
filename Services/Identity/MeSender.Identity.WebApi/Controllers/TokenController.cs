using MeSender.Identity.Services;
using MeSender.Identity.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeSender.Identity.WebApi.Controllers;

[ApiController]
[Route("api/token/")]
public sealed class TokenController(IUserService userService) : ControllerBase
{
    [HttpPost("refresh")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        var result = await userService.RefreshUserTokenAsync(refreshToken);
        if (result.IsFailure)
        {
            return Problem(
                detail: "The provided refresh token was not found or has expired",
                statusCode: StatusCodes.Status404NotFound,
                title: "Invalid or expired refresh token");
        }

        return Ok(new TokenResponse
        {
            AccessToken = result.Value.AccessToken,
            RefreshToken = result.Value.RefreshToken,
            AccessTokenExpiresAt = result.Value.AccessTokenExpiresAt,
        });
    }
}
