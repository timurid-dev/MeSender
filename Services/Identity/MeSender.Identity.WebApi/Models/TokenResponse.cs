using System.Text.Json.Serialization;

namespace MeSender.Identity.WebApi.Models;

public sealed class TokenResponse
{
    public required string AccessToken { get; init; }

    public required DateTimeOffset AccessTokenExpiresAt { get; init; }

    public required string RefreshToken { get; init; }
}
