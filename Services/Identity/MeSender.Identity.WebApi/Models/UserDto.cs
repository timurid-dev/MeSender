namespace MeSender.Identity.WebApi.Models;

public sealed record UserDto
{
    public required string Email { get; init; }

    public required string Password { get; init; }
}
