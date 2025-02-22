using System.ComponentModel.DataAnnotations;

namespace MeSender.Messages.WebApi.Models;

public class Message
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string Text { get; set; } = string.Empty;
}