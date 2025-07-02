using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MeSender.Identity.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MeSender.Identity.ComponentTests;

public sealed class LoginControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task LoginUser_WithValidCredentials_ShouldReturnTokens()
    {
        // Arrange
        var user = new UserDto("login@example.com", "password123");
        await _client.PutAsJsonAsync("api/register/", user);

        // Act
        var response = await _client.PostAsJsonAsync("api/login/", user);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        tokenResponse.Should().NotBeNull();
        tokenResponse!.AccessToken.Should().NotBeNullOrEmpty();
        tokenResponse.RefreshToken.Should().NotBeNullOrEmpty();
        tokenResponse.AccessTokenExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task LoginUser_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var user = new UserDto("notfound@example.com", "password123");

        // Act
        var response = await _client.PostAsJsonAsync("api/login/", user);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Title.Should().Be("Authentication failed");
        problemDetails.Detail.Should().Be("Invalid username or password");

        // Arrange: зарегистрируем пользователя, но с другим паролем
        var regUser = new UserDto("login@example.com", "password123");
        await _client.PutAsJsonAsync("api/register/", regUser);
        var wrongPasswordUser = new UserDto("login@example.com", "wrongpassword");

        // Act
        var response2 = await _client.PostAsJsonAsync("api/login/", wrongPasswordUser);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var problemDetails2 = await response2.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails2.Should().NotBeNull();
        problemDetails2!.Title.Should().Be("Authentication failed");
        problemDetails2.Detail.Should().Be("Invalid username or password");
    }

    async Task IAsyncLifetime.InitializeAsync() => await factory.ResetDatabaseAsync();

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
}
