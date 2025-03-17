namespace ChatRoom.Models;

internal record Message(string Text)
{
    public DateTimeOffset CreateTimestamp { get; init; }

    public DateTimeOffset? UpdatedTimeStamp { get; init; }
}
