﻿namespace MeSender.Identity.Models;

internal sealed class AuthData
{
    public Guid Id { get; init; }

    public required string Password { get; init; }

    public required string Salt { get; init; }
}
