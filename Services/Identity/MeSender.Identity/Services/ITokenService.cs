using MeSender.Identity.Models;

namespace MeSender.Identity.Services;

public interface ITokenService
{
    public TokenPair GenerateTokens(Guid userId, string email);

    public Task<TokenPair?> RefreshTokensAsync(string email, string refreshToken);
}
