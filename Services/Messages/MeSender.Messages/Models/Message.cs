using System.ComponentModel.DataAnnotations;

namespace MeSender.Messages.Models;

internal sealed class Message
{
    public Guid Id { get; init; }

    [MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    public DateTimeOffset CreateTimestamp { get; init; }

    public DateTimeOffset? UpdateTimestamp { get; set; }
}
