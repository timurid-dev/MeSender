namespace MeSender.Identity.WebApi.Models;

public sealed record RefreshTokenDto(string Email, string RefreshToken);
