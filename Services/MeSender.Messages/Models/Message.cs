using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MeSender.Messages.Models;

public record Message
{
    [JsonIgnore]
    public int Id { get; init; }

    [Required]
    [MaxLength(1000)]
    public string Text { get; init; } = string.Empty;
}
