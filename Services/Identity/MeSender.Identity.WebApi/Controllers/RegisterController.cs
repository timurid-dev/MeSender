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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterUser([FromBody] UserDto user)
    {
        var result = await userService.AddUserAsync(user.Email, user.Password);
        if (result.IsFailure)
        {
            return Problem(
                detail: "The user with this email is already registered",
                statusCode: StatusCodes.Status409Conflict,
                title: "User already exists");
        }

        return Ok(new
        {
            Message = "User registered successfully.",
        });
    }
}
