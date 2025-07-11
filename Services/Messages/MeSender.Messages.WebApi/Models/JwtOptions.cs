namespace MeSender.Messages.WebApi.Models;

public sealed class JwtOptions
{
    public static string SectionName => "Jwt";

    public required string Secret { get; init; }

    public required string Issuer { get; init; }

    public required string Audience { get; init; }
}
