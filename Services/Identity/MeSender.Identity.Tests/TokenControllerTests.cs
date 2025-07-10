using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MeSender.Identity.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MeSender.Identity.Tests;

public sealed class TokenControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task RefreshToken_WithValidToken_ShouldReturnNewTokens()
    {
        // Arrange
        var user = new UserDto
        {
            Email = "refresh@example.com",
            Password = "password123",
        };
        await _client.PutAsJsonAsync("api/register/", user);
        var loginResponse = await _client.PostAsJsonAsync("api/login/", user);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();

        // Act
        var response = await _client.PostAsJsonAsync("api/token/refresh", loginResult!.RefreshToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        tokenResponse.Should().NotBeNull();
        tokenResponse.AccessToken.Should().NotBeNullOrEmpty();
        tokenResponse.RefreshToken.Should().NotBeNullOrEmpty();
        tokenResponse.AccessTokenExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
        tokenResponse.AccessToken.Should().NotBe(loginResult.AccessToken);
        tokenResponse.RefreshToken.Should().NotBe(loginResult.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidOrExpiredToken_ShouldReturnNotFound()
    {
        // Arrange
        var user = new UserDto
        {
            Email = "refresh@example.com",
            Password = "password123",
        };
        await _client.PutAsJsonAsync("api/register/", user);
        await _client.PostAsJsonAsync("api/login/", user);

        // Act
        var response = await _client.PostAsJsonAsync("api/token/refresh", "invalid-refresh-token");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails.Title.Should().Be("Invalid or expired refresh token");
        problemDetails.Detail.Should().Be("The provided refresh token was not found or has expired");
    }

    async Task IAsyncLifetime.InitializeAsync() => await factory.ResetDatabaseAsync();

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
}
