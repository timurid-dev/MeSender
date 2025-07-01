using MeSender.Identity.Services;
using MeSender.Identity.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeSender.Identity.WebApi.Controllers;

[ApiController]
[Route("api/token/")]
public sealed class TokenController(ITokenService tokenService) : ControllerBase
{
    [HttpPost("refresh")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        var tokenPair = await tokenService.RefreshTokensAsync(refreshTokenDto.Email, refreshTokenDto.RefreshToken);
        if (tokenPair == null)
        {
            return Unauthorized(new
            {
                Message = "Invalid or expired refresh token",
            });
        }

        return Ok(new TokenResponse
        {
            AccessToken = tokenPair.AccessToken,
            RefreshToken = tokenPair.RefreshToken,
            ExpiresAt = tokenPair.AccessTokenExpiresAt,
        });
    }
}
