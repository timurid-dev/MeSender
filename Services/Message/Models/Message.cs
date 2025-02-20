using System.ComponentModel.DataAnnotations;

namespace Message.Models;

public class Message
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string Text { get; set; } = string.Empty;
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}