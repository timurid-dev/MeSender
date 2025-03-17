namespace MeSender.Messages.Models;

public record MessageDto
{
    public Guid Id { get; init; }

    public string Text { get; set; } = string.Empty;

    public DateTimeOffset CreateTimestamp { get; init; }

    public DateTimeOffset? UpdateTimestamp { get; init; }
}
