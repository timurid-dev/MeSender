namespace ChatRoom.Models;

internal record Message
{
    public string Text { get; init; } = string.Empty;
}
