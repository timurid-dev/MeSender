using System.Net.Http.Json;
using FluentAssertions;
using MeSender.Messages.Models;
using Xunit;

namespace MeSender.Messages.Tests;

public sealed class MessagesControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetMessages_ShouldReturnEmptyList()
    {
        // Arrange

        // Act
         var messages = await _client.GetFromJsonAsync<List<Message>>("api/messages/");

        // Assert
         messages.Should().BeEmpty();
    }

    [Fact]
    public async Task SendMessage_ShouldAppearInList()
    {
        // Arrange
        var message = new Message
        {
            Text = "Test Message",
        };

        // Act
        var sendResponse = await _client.PutAsJsonAsync($"/api/messages/{Guid.NewGuid()}", message);
        sendResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        var messages = await _client.GetFromJsonAsync<List<Message>>("api/messages/");

        // Assert
        messages.Should().ContainSingle();
        messages[0].Text.Should().Be("Test Message");
    }

    [Fact]
    public async Task SendTwoMessages_ShouldAppearInOrder()
    {
        // Arrange
        var message1 = new Message
            {
                Text = "First Message",
            };
        var message2 = new Message
        {
            Text = "Second Message",
        };

        // Act
        await _client.PutAsJsonAsync($"/api/messages/{Guid.NewGuid()}", message1);
        await _client.PutAsJsonAsync($"/api/messages/{Guid.NewGuid()}", message2);
        var messages = await _client.GetFromJsonAsync<List<Message>>("api/messages/");

        // Assert
        messages.Should().HaveCount(2);
        messages[0].Text.Should().Be("First Message");
        messages[1].Text.Should().Be("Second Message");
    }

    [Fact]
    public async Task UpdateMessage_ShouldUpdateTextAndTimestamp()
    {
        // Arrange
        var message = new MessageDto
        {
            Text = "Original Text",
        };
        var messageId = Guid.NewGuid();
        const string uri = "/api/messages/";
        await _client.PutAsJsonAsync($"{uri}{messageId}", message);

        // Act
        message.Text = "Updated Text";
        await _client.PutAsJsonAsync($"{uri}{messageId}", message);

        // Assert
        var updatedMessages = await _client.GetFromJsonAsync<List<MessageDto>>(uri);
        updatedMessages.Should().ContainSingle();
        var updatedMessage = updatedMessages[0];
        updatedMessage.Text.Should().Be("Updated Text");
        if (updatedMessage.UpdateTimestamp != null)
        {
            updatedMessage.UpdateTimestamp.Should().BeAfter(updatedMessage.CreateTimestamp);
        }
    }

    async Task IAsyncLifetime.InitializeAsync() => await factory.ClearDatabaseAsync();

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
}
