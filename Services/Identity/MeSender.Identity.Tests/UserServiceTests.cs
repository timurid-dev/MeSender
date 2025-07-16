using MeSender.Identity.Models;
using MeSender.Identity.Repositories;
using MeSender.Identity.Services;
using Moq;
using Xunit;

namespace MeSender.Identity.Tests;

public sealed class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<IPasswordService> _passwordService = new();
    private readonly TimeProvider _timeProvider = TimeProvider.System;

    private UserService CreateService() => new(
        _userRepository.Object,
        _tokenService.Object,
        _timeProvider,
        _passwordService.Object);

    [Fact]
    public async Task LoginUserAsync_UserFoundAndPasswordCorrect_ReturnsTokenPair()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password";
        var userId = Guid.NewGuid();
        var authData = new AuthData
            {
                UserId = userId, Password = "hash", Salt = "salt",
            };
        var tokenPair = new TokenPair
        {
            AccessToken = "access",
            RefreshToken = "refresh",
            AccessTokenExpiresAt = DateTimeOffset.UtcNow.AddMinutes(15),
            RefreshTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
        };
        _userRepository.Setup(r => r.LoginUserAsync(email)).ReturnsAsync(authData);
        _passwordService.Setup(p => p.VerifyPassword(password, authData.Password, authData.Salt)).Returns(value: true);
        _tokenService.Setup(t => t.GenerateTokens(userId)).Returns(tokenPair);
        _userRepository.Setup(r => r.AddRefreshTokenAsync(userId, tokenPair.RefreshToken, tokenPair.RefreshTokenExpiresAt)).Returns(Task.CompletedTask);
        var service = CreateService();

        // Act
        var result = await service.LoginUserAsync(email, password, "provider");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(tokenPair, result.Value);
        _userRepository.Verify(r => r.LoginUserAsync(email), Times.Once);
        _passwordService.Verify(p => p.VerifyPassword(password, authData.Password, authData.Salt), Times.Once);
        _tokenService.Verify(t => t.GenerateTokens(userId), Times.Once);
        _userRepository.Verify(r => r.AddRefreshTokenAsync(userId, tokenPair.RefreshToken, tokenPair.RefreshTokenExpiresAt), Times.Once);
    }

    [Fact]
    public async Task LoginUserAsync_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var email = "notfound@example.com";
        _userRepository.Setup(r => r.LoginUserAsync(email)).ReturnsAsync((AuthData?)null);
        var service = CreateService();

        // Act
        var result = await service.LoginUserAsync(email, "any", "provider");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Invalid email", result.Error);
        _userRepository.Verify(r => r.LoginUserAsync(email), Times.Once);
        _passwordService.Verify(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _tokenService.Verify(t => t.GenerateTokens(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task LoginUserAsync_PasswordIncorrect_ReturnsFailure()
    {
        // Arrange
        var email = "test@example.com";
        var password = "wrong";
        var authData = new AuthData
        {
            UserId = Guid.NewGuid(), Password = "hash", Salt = "salt",
        };
        _userRepository.Setup(r => r.LoginUserAsync(email)).ReturnsAsync(authData);
        _passwordService.Setup(p => p.VerifyPassword(password, authData.Password, authData.Salt)).Returns(value: false);
        var service = CreateService();

        // Act
        var result = await service.LoginUserAsync(email, password, "provider");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Invalid password", result.Error);
        _userRepository.Verify(r => r.LoginUserAsync(email), Times.Once);
        _passwordService.Verify(p => p.VerifyPassword(password, authData.Password, authData.Salt), Times.Once);
        _tokenService.Verify(t => t.GenerateTokens(It.IsAny<Guid>()), Times.Never);
    }
}
