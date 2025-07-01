namespace MeSender.Identity.Models;

public sealed class RefreshTokenData
{
    public Guid UserId { get; init; }

    public required string RefreshToken { get; init; }

    public required DateTimeOffset RefreshTokenExpiresAt { get; init; }
}
