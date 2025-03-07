namespace MeSender.Messages.WebApi.Models;

public record Message
{
    public Guid Id { get; init; } = Guid.Empty;

    public string Text { get; init; } = string.Empty;

    public DateTime? CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}
