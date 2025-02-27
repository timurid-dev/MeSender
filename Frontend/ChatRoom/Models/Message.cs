namespace ChatRoom.Models;

internal record Message
{
    public Guid Id { get; init; }

    public string Text { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }
}
