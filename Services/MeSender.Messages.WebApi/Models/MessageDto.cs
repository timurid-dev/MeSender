namespace MeSender.Messages.WebApi.Models;

public record MessageDto
{
    public Guid Id { get; init; } = Guid.Empty;

    public string Text { get; init; } = string.Empty;

    public DateTime? CreatedAt { get; init; } = DateTime.MinValue;

    public DateTime? UpdatedAt { get; init; }
}
