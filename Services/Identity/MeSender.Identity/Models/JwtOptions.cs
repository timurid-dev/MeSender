namespace MeSender.Identity.Models;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string Secret { get; init; }

    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    public TimeSpan AccessTokenExpirationSpan { get; init; }

    public TimeSpan RefreshTokenExpirationSpan { get; init; }
}
