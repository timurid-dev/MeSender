using MeSender.Identity.Services;
using MeSender.Identity.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeSender.Identity.WebApi.Controllers;

[ApiController]
[Route("api/login/")]
public sealed class LoginController(IUserService userService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginUser([FromBody] UserDto user)
    {
        var (accessToken, refreshToken, expiresAt) = await userService.LoginUserAsync(user.Email, user.Password);

        if (accessToken.Length == 0)
        {
            return Unauthorized(new
            {
                Message = "User or password is invalid",
            });
        }

        var response = new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
        };

        return Ok(response);
    }
}
