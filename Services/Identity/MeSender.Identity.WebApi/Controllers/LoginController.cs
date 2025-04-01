using MeSender.Identity.Services;
using MeSender.Identity.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeSender.Identity.WebApi.Controllers;

[ApiController]
[Route("api/login/")]
public sealed class LoginController(IUserService userService) : ControllerBase
{
    [HttpPut]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginUser([FromBody] UserDto user)
    {
        var isSuccess = await userService.LoginUserAsync(user.Email, user.Password);

        if (isSuccess)
        {
            return Ok(new
            {
                Message = "Login is successfully",
            });
        }

        return Unauthorized(new
        {
            Message = "User or password is invalid",
        });
    }
}
