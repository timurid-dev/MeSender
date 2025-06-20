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
        var tokenPair = await userService.LoginUserAsync(user.Email, user.Password);

        if (tokenPair == null)
        {
            return Unauthorized(new
            {
                Message = "User or password is invalid",
            });
        }

        return Ok(tokenPair);
    }
}
