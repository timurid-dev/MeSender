namespace MeSender.Identity.Models;

internal sealed class UserEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string Email { get; init; }

    public required string Password { get; init; }
}
