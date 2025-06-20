using MeSender.Identity.Services;
using MeSender.Identity.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeSender.Identity.WebApi.Controllers;

[ApiController]
[Route("api/register/")]
public sealed class RegisterController(IUserService userService) : ControllerBase
{
    [HttpPut]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterUser([FromBody] UserDto user)
    {
        var isSuccess = await userService.AddUserAsync(user.Email, user.Password);

        if (isSuccess)
        {
            return Ok(new
            {
                Message = "User registered successfully.",
            });
        }

        return Conflict(new
        {
            Message = "Not registered, because user exists",
        });
    }
}
