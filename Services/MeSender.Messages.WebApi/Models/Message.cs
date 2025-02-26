namespace MeSender.Messages.WebApi.Models;

public record Message
{
    public string Text { get; init; } = string.Empty;
}
