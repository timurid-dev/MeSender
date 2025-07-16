namespace MeSender.Identity.Models;

public sealed class PasswordData
{
    public required string PasswordHash { get; init; }

    public required string Salt { get; init; }
}
