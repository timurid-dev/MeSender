namespace MeSender.Identity.Models;

public sealed class UserEntity
{
    public Guid Id { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }

    public required string Salt { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}
