using System.ComponentModel.DataAnnotations;

namespace MeSender.Messages.Models;

public sealed class Message
{
    public Guid Id { get; init; }

    [MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.MinValue;

    public DateTime UpdatedAt { get; set; }
}
