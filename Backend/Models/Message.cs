using System.ComponentModel.DataAnnotations;

namespace eBarber.Models;

public class Message
{
    public int Id { get; set; }
    
    [Required]
    public string SenderId { get; set; } = string.Empty;
    public ApplicationUser? Sender { get; set; }
    
    public string? ReceiverId { get; set; }
    public ApplicationUser? Receiver { get; set; }
    
    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;
    
    public DateTime SentAt { get; set; } = DateTime.Now;
    
    public bool IsRead { get; set; } = false;
    
    [StringLength(50)]
    public string MessageType { get; set; } = "Text";
}