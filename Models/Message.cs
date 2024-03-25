using System.ComponentModel.DataAnnotations;
using COMP1640.Areas.Identity.Data;
using COMP1640.Models;

namespace COMP1640.Models;
public class Message
{
    [Key]
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }

    // Foreign key
    public int ChatId { get; set; }
    // Navigation property
    public Chat Chat { get; set; }

    public string UserId { get; set; }
    public COMP1640User? User { get; set; }
}