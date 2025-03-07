using System.Net.Http.Json;
using FluentAssertions;
using MeSender.Messages.Data;
using MeSender.Messages.WebApi.Models;
using Microsoft.Extensions.DependencyInjection;
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
        var sendResponse = await _client.PostAsJsonAsync("/api/messages", message);
        sendResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var getResponse = await _client.GetAsync("/api/messages");
        var messages = await getResponse.Content.ReadFromJsonAsync<List<Message>>();

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
        await _client.PostAsJsonAsync("/api/messages", message1);
        await _client.PostAsJsonAsync("/api/messages", message2);

        var getResponse = await _client.GetAsync("/api/messages");
        var messages = await getResponse.Content.ReadFromJsonAsync<List<Message>>();

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
        await _client.PostAsJsonAsync("/api/messages", message);

        var getResponse = await _client.GetAsync("/api/messages");
        var originalMessage = (await getResponse.Content.ReadFromJsonAsync<List<Message>>())?[0];

        if (originalMessage != null)
        {
            var updatedMessage = new Message
                {
                    Id = originalMessage.Id, Text = "Updated Text",
                };

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/api/messages/{originalMessage.Id}", updatedMessage);
            updateResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        var updatedGetResponse = await _client.GetAsync("/api/messages");
        var updatedMessages = await updatedGetResponse.Content.ReadFromJsonAsync<List<Message>>();

        // Assert
        updatedMessages.Should().ContainSingle();
        updatedMessages[0].Text.Should().Be("Updated Text");
        if (originalMessage != null)
        {
            updatedMessages[0].UpdatedAt.Should().BeAfter(originalMessage.UpdatedAt);
        }
    }
}
