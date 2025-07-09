using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MeSender.Identity.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MeSender.Identity.Tests;

public sealed class RegisterControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task RegisterUser_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var user = new UserDto("test@example.com", "password123");

        // Act
        var response = await _client.PutAsJsonAsync("api/register/", user);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterUser_WithDuplicateEmail_ShouldReturnConflict()
    {
        // Arrange
        var user = new UserDto("duplicate@example.com", "password123");
        await _client.PutAsJsonAsync("api/register/", user);

        // Act
        var response = await _client.PutAsJsonAsync("api/register/", user);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails.Title.Should().Be("User already exists");
        problemDetails.Detail.Should().Be("The user with this email is already registered");
    }

    async Task IAsyncLifetime.InitializeAsync() => await factory.ResetDatabaseAsync();

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
}
