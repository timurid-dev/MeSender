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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginUser([FromBody] UserDto user)
    {
        var provider = "swagger";
        var result = await userService.LoginUserAsync(user.Email, user.Password, provider);
        if (result.IsFailure)
        {
            return Problem(
                detail: "Invalid username or password",
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Authentication failed");
        }

        return Ok(result.Value);
    }
}
