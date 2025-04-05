namespace MeSender.Identity.Models;

internal sealed class TokenPair
{
    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }

    public required DateTimeOffset ExpiresAt { get; init; }
}
