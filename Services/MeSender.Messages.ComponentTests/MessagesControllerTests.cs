using System.Net.Http.Json;
using FluentAssertions;
using MeSender.Messages.Models;
using Xunit;

namespace MeSender.Messages.ComponentTests;

public sealed class MessagesControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetMessages_ShouldReturnEmptyList()
    {
        // Arrange
         factory.ClearDatabase();

        // Act
         var response = await _client.GetAsync("/api/messages");
         var messages = await response.Content.ReadFromJsonAsync<List<Message>>();

        // Assert
         response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
         messages.Should().BeEmpty();
    }

    [Fact]
    public async Task SendMessage_ShouldAppearInList()
    {
        // Arrange
        factory.ClearDatabase();
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
        factory.ClearDatabase();
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
        factory.ClearDatabase();
        var message = new Message
        {
            Text = "Original Text",
        };
        var messageId = Guid.NewGuid();

        await _client.PutAsJsonAsync($"/api/messages/{messageId}", message);
        var messages = await _client.GetFromJsonAsync<List<Message>>("api/messages/");

        // Act
        if (messages?[0] != null)
        {
            messages[0].Text = "Updated Text";
            await _client.PutAsJsonAsync($"/api/messages/{messages[0].Id}", messages[0]);
        }

        var updatedMessages = await _client.GetFromJsonAsync<List<Message>>("api/messages/");

        // Assert
        updatedMessages.Should().ContainSingle();
        updatedMessages[0].Text.Should().Be("Updated Text");
        if (messages?[0].UpdatedAt != null)
        {
            updatedMessages[0].UpdatedAt.Should().BeAfter(messages[0].UpdatedAt);
        }
    }
}
