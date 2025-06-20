namespace MeSender.Identity.Models;

public sealed class TokenPair
{
    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }

    public required DateTimeOffset AccessTokenExpiresAt { get; init; }
}
