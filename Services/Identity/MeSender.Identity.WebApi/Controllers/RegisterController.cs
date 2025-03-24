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
    public IActionResult RegisterUser([FromBody] UserDto user)
    {
        userService.AddUser(user.Email, user.Password);
        return Ok(new
            {
                Message = "User registered successfully.",
            });
    }
}
