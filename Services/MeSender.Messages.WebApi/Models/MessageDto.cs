namespace MeSender.Messages.WebApi.Models;

public record MessageDto
{
    public string Text { get; init; } = string.Empty;
}
