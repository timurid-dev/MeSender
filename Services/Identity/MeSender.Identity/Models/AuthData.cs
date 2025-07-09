namespace MeSender.Identity.Models;

public sealed class AuthData
{
    public Guid UserId { get; init; }

    public required string Password { get; init; }

    public required string Salt { get; init; }
}
