﻿namespace MeSender.Identity.Models;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string Secret { get; init; }

    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    public int AccessTokenExpirationMinutes { get; init; } = 15;
}
