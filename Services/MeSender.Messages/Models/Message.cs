using System.ComponentModel.DataAnnotations;

namespace MeSender.Messages.Models;

public sealed class Message
{
    public int Id { get; init; }

    [MaxLength(1000)]
    public string Text { get; init; } = string.Empty;
}
